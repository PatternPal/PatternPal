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
                new ElementCheck<IEntityNode>(x => x.CheckModifier("Abstract"), new ResourceMessage("DecoratorNodeModifier")),
                new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.Extends, 1) || x.CheckMinimalAmountOfRelationTypes(RelationType.Implements, 1), new ResourceMessage("DecoratorNodeParent")),
                new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.ExtendedBy, 1), new ResourceMessage("DecoratorNodeChild")),
                
                //Component checks
                new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>{
                    new ElementCheck<IEntityNode>(x => {currentComponent = x; return x.GetMethods().Any(); }, new ResourceMessage("DecoratorComponentMethodAny")),
                    new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.ExtendedBy, 2) || x.CheckMinimalAmountOfRelationTypes(RelationType.ImplementedBy, 2), new ResourceMessage("DecoratorComponentChild")),

                    new GroupCheck<IEntityNode, IMethod>( new List<ICheck<IMethod>>{
                        new ElementCheck<IMethod>(x => x.CheckModifier("public") || x.CheckModifier("protected") , new ResourceMessage("DecoratorComponentMethodModifier")),
                        new ElementCheck<IMethod>(x => x.CheckParameters(new List<string>{currentComponent.GetName() }), new ResourceMessage("DecoratorComponentMethodParameters", new []{ currentComponent.GetName()}))

                    }, x => entityNode.GetConstructors(), "Constructor"),
                    new GroupCheck<IEntityNode, IField>( new List<ICheck<IField>>{
                        new ElementCheck<IField>(x => x.CheckFieldType(new List<string>{currentComponent.GetName() }) , new ResourceMessage("DecoratorComponentFieldType", new []{ currentComponent.GetName()}))

                    }, x => entityNode.GetFields(), "Field"),

                    //Concrete decorator
                    new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>{
                        new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.CheckModifier("public"), new ResourceMessage("DecoratorConcreteMethodModifier")),
                            new ElementCheck<IMethod>(x => x.CheckParameters(new List<string>() { currentComponent.GetName() }), new ResourceMessage("DecoratorConcreteMethodParameters", new[]{currentComponent.GetName()})),
                            new ElementCheck<IMethod>(x => x.CheckArguments(currentComponent.GetName()), new ResourceMessage("DecoratorConcreteMethodArguments"))

                        }, x => x.GetConstructors(), "Constructor")
                    }, x => entityNode.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.ExtendedBy)).Select(y => y.GetDestination()), new ResourceMessage("DecoratorConcrete"), GroupCheckType.Median)


                }, x => x.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.Implements) || y.GetRelationType().Equals(RelationType.Extends)).Select(y => y.GetDestination()), new ResourceMessage("DecoratorComponent"))

            }, x => new List<IEntityNode>() { entityNode },new ResourceMessage("Decorator"));
            var checkResult = decoratorCheck.Check(entityNode);

            result.Results = checkResult.GetChildFeedback().ToList();
            return result;
        }
    }
}
