using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models;
using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;

namespace IDesign.Recognizers
{
    public class DecoratorRecognizer : IRecognizer
    {
        /// <summary>
        ///     Function thats checks the returntype of a method.
        /// </summary>
        /// <param name="entityNode">The method witch it should check</param>
        /// <returns></returns>
        public IResult Recognize(IEntityNode entityNode)
        {
            Result result = new Result();

            IEntityNode currentComponent = null;
            var decoratorCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>()
            {
                new ElementCheck<IEntityNode>(x => x.CheckModifier("Abstract"), "NodeModifierAbstract"),
                new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.Extends, 1) || x.CheckMinimalAmountOfRelationTypes(RelationType.Implements, 1), new ResourceMessage("Parent")),
                new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.ExtendedBy, 1), "Child"),
                
                //Component checks
                new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>{
                    new ElementCheck<IEntityNode>(x => {currentComponent = x; return x.GetMethods().Any(); }, "MethodAny"),
                    new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.ExtendedBy, 2) || x.CheckMinimalAmountOfRelationTypes(RelationType.ImplementedBy, 2), "DecoratorComponentChild"),

                    new GroupCheck<IEntityNode, IMethod>( new List<ICheck<IMethod>>{
                        new ElementCheck<IMethod>(x => x.CheckModifier("public") || x.CheckModifier("protected") , "MethodModifierNotPrivate"),
                        new ElementCheck<IMethod>(x => x.CheckParameters(new List<string>{currentComponent.GetName() }), "DecoratorComponentMethodParameters")

                    }, x => entityNode.GetConstructors(), "DecoratorComponentConstructor"),
                    new GroupCheck<IEntityNode, IField>( new List<ICheck<IField>>{
                        new ElementCheck<IField>(x => x.CheckFieldType(new List<string>{currentComponent.GetName() }) , "DecoratorComponentFieldType")

                    }, x => entityNode.GetFields(), "DecoratorComponentField"),

                    //Concrete decorator
                    new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>{
                        new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.CheckModifier("public"), "MethodModifierPublic"),
                            new ElementCheck<IMethod>(x => x.CheckParameters(new List<string>() { currentComponent.GetName() }), "ConcreteDecoratorMethodParameters"),
                            new ElementCheck<IMethod>(x => x.CheckArguments(currentComponent.GetName()), "DecoratorConcreteMethodArguments")

                        }, x => x.GetConstructors(), "ConcreteDecoratorConstructor")
                    }, x => entityNode.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.ExtendedBy)).Select(y => y.GetDestination()), "DecoratorConcrete", GroupCheckType.Median)


                }, x => x.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.Implements) || y.GetRelationType().Equals(RelationType.Extends)).Select(y => y.GetDestination()), "DecoratorComponent")

            }, x => new List<IEntityNode>() { entityNode },"Decorator");
            var checkResult = decoratorCheck.Check(entityNode);

            result.Results = checkResult.GetChildFeedback().ToList();
            return result;
        }
    }
}
