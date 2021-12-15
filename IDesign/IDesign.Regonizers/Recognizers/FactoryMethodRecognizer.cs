using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Recognizers
{
    public class FactoryMethodRecognizer : IRecognizer
    {
        private Result result;

        public IResult Recognize(IEntity node)
        {
            result = new Result();
            IEntity entityNode = null;
            IEntity productnode = null;
            var factoryMethodChecks = new GroupCheck<IEntity, IEntity>(new List<ICheck<IEntity>>
            {
                //check if node is abstract class (creator)
                new ElementCheck<IEntity>(x => x.CheckTypeDeclaration(EntityType.Class) && x.CheckModifier("abstract"),
                    "NodeModifierAbstract", 2),
                new ElementCheck<IEntity>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.Uses, 1), "NodeUses1",
                    1),

                //check concretecreators
                new GroupCheck<IEntity, IEntity>(new List<ICheck<IEntity>>
                {
                    //concrete creator
                    new ElementCheck<IEntity>(x =>
                    {
                        entityNode = x;
                        return x.GetAllMethods().Any();
                    }, "FactoryConcreteCreatorMethodAny"),

                    //check if node (concrete creator) has creates relations
                    new GroupCheck<IEntity, IEntity>(new List<ICheck<IEntity>>
                    {
                        //concrete product
                        new ElementCheck<IEntity>(x =>
                        {
                            productnode = x;
                            return x.GetAllMethods().Any();
                        }, "FactoryConcreteProductMethodAny"),

                        //check if method in node (concrete creator) creates new node (concrete product)
                        new GroupCheck<IEntity, IMethod>(
                            new List<ICheck<IMethod>>
                            {
                                new ElementCheck<IMethod>(x => x.CheckCreationType(productnode.GetName()),
                                    "FactoryMethodCreateTypeProduct", 2)
                            }, x => entityNode.GetAllMethods(), "FactoryAbstractCreatorMethod"),

                        //check if node (concrete product) implements/extends a node (product)
                        new GroupCheck<IEntity, IEntity>(new List<ICheck<IEntity>>
                        {
                            //product
                            new ElementCheck<IEntity>(x =>
                            {
                                productnode = x;
                                return x.GetAllMethods().Any() || x.GetFields().Any();
                            }, "ProductClass"),
                            new GroupCheck<IEntity, IMethod>(
                                new List<ICheck<IMethod>>
                                {
                                    new ElementCheck<IMethod>(x => x.CheckReturnType(productnode.GetName()),
                                        "FactoryMethodCreateTypeProduct", 2)
                                }, x => node.GetAllMethods(), "FactoryAbstractCreatorMethod")
                        }, x => x.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.Extends) ||
                                                            y.GetRelationType().Equals(RelationType.Implements))
                            .Select(y => y.GetDestination()), "Child")
                    }, x => entityNode.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.Creates))
                        .Select(y => y.GetDestination()), "FactoryCreates")
                }, x => x.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.ExtendedBy))
                    .Select(y => y.GetDestination()), "FactoryConcreteCreator", GroupCheckType.All),

                //product node
                new GroupCheck<IEntity, IEntity>(new List<ICheck<IEntity>>
                {
                    //product
                    new ElementCheck<IEntity>(x =>
                    {
                        entityNode = x;
                        return x.GetAllMethods().Any() || x.GetFields().Any();
                    }, "ProductClassNotEmpty"),

                    //check if node (creator) has uses relations (method) with return type of interface node (product)
                    new GroupCheck<IEntity, IMethod>(
                        new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.CheckModifier("public"), "MethodModifierPublic", 0.5f),
                            new ElementCheck<IMethod>(x => x.CheckModifier("abstract"), "MethodModifierAbstract",
                                1),
                            new ElementCheck<IMethod>(x => x.CheckReturnType(entityNode.GetName()),
                                "FactoryMethodReturnTypeProductInterface", 2)
                        }, x => node.GetAllMethods(), "FactoryConcreteCreatorMethod")
                }, x => x.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.Uses))
                    .Select(y => y.GetDestination()), "ProductClass")
            }, x => new List<IEntity> {node}, "FactoryAbstractCreator");

            result.Results.Add(factoryMethodChecks.Check(node));
            return result;
        }
    }
}
