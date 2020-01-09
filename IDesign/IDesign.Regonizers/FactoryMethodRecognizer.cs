using System;
using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers
{
    public class FactoryMethodRecognizer : IRecognizer
    {
        Result result;
        public IResult Recognize(IEntityNode node)
        {
            result = new Result();
            IEntityNode entityNode = null;
            IEntityNode productnode = null;
            var factoryMethodChecks = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                //check if node is abstract class (creator)
                new ElementCheck<IEntityNode>(x => x.CheckTypeDeclaration(EntityNodeType.Class)&& x.CheckModifier("abstract"),"creator should be an abstract class",2),
                new ElementCheck<IEntityNode>(x=> x.CheckMinimalAmountOfRelationTypes(RelationType.Uses,1),"Node should have min 1 uses relation",1),

                //check concretecreators
                new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                {
                    //concrete creator
                    new ElementCheck<IEntityNode>(x => {entityNode = x; return x.GetMethodsAndProperties().Any(); }, "concrete creator"),

                    //check if node (concrete creator) has creates relations
                    new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                    {
                        //concrete product
                        new ElementCheck<IEntityNode>(x => {productnode = x; return x.GetMethodsAndProperties().Any(); }, "concrete product"),

                        //check if method in node (concrete creator) creates new node (concrete product)
                        new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.CheckCreationType(productnode.GetName()), $"should create new conrete product",2),
                        }, x => entityNode.GetMethodsAndProperties(), "method"),

                        //check if node (concrete product) implements/extends a node (product)
                        new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                        {
                            //product
                            new ElementCheck<IEntityNode>(x => {productnode = x; return x.GetMethodsAndProperties().Any() || x.GetFields().Any(); }, "product"),

                            new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                            {
                                new ElementCheck<IMethod>(x => x.CheckReturnType(productnode.GetName()), $"return type should be equal to the product",2),
                            }, x => node.GetMethodsAndProperties(), "method")

                        }, x => x.GetRelations().Where(y => (y.GetRelationType().Equals(RelationType.Extends)) ||
                        (y.GetRelationType().Equals(RelationType.Implements)))
                        .Select(y => y.GetDestination()),"implements/extends"),

                    }, x=> entityNode.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.Creates))
                    .Select(y => y.GetDestination()), $"Creates")

                }, x => x.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.ExtendedBy))
                .Select(y => y.GetDestination()), $"{node.GetName()} is parent of: ", GroupCheckType.All),

                //product node
                new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                {
                    //product
                    new ElementCheck<IEntityNode>(x => {entityNode = x; return x.GetMethodsAndProperties().Any() || x.GetFields().Any(); }, "product"),

                    //check if node (creator) has uses relations (method) with return type of interface node (product)
                    new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                    {

                         new ElementCheck<IMethod>(x => x.CheckModifier("public"),"modifier should be public",0.5f),
                         new ElementCheck<IMethod>(x => x.CheckModifier("abstract"),"modifier should be abstract",1),
                         new ElementCheck<IMethod>(x => x.CheckReturnType(entityNode.GetName()),"Return type should be equal to the product",2)
                    }, x=> node.GetMethodsAndProperties(), "Methods")

                }, x=> x.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.Uses))
                .Select(y => y.GetDestination()),"Uses relations", GroupCheckType.All)

            }, x => new List<IEntityNode> { node }, "Factory method");

            result.Results.Add(factoryMethodChecks.Check(node));
            return result;
        }
    }
}
