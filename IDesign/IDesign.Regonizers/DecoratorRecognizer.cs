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
                new ElementCheck<IEntityNode>(x => x.CheckModifier("Abstract"), "Is abstract"),
                new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.Extends, 1) || x.CheckMinimalAmountOfRelationTypes(RelationType.Implements, 1), "Has an interface or an parent class"),
                new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.ExtendedBy, 1), "Extends a different class"),
                
                //Component checks
                new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>{
                    new ElementCheck<IEntityNode>(x => {currentComponent = x; return x.GetMethods().Any(); }, "Has functions"),
                    new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.ExtendedBy, 2) || x.CheckMinimalAmountOfRelationTypes(RelationType.ImplementedBy, 2), "Extended or implemented by a different class"),

                    new GroupCheck<IEntityNode, IMethod>( new List<ICheck<IMethod>>{
                        new ElementCheck<IMethod>(x => x.CheckModifier("public") || x.CheckModifier("protected") , "Is public or protected"),
                        new ElementCheck<IMethod>(x => x.CheckParameters(new List<string>{currentComponent.GetName() }), "Has the interface of an parent class as parameter")

                    }, x => entityNode.GetConstructors(), "Constructor"),
                    new GroupCheck<IEntityNode, IField>( new List<ICheck<IField>>{
                        new ElementCheck<IField>(x => x.CheckFieldType(new List<string>{currentComponent.GetName() }) , "Has the interface saved as a property or field")

                    }, x => entityNode.GetFields(), "Field"),

                    //Concrete decorator
                    new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>{
                        new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.CheckModifier("public"), "Is public or protected"),
                            new ElementCheck<IMethod>(x => x.CheckParameters(new List<string>() { currentComponent.GetName() }), "Has the interface or parent class as parameter"),
                            new ElementCheck<IMethod>(x => x.CheckArguments(currentComponent.GetName()), "Sends the interface or parent class parameter in base")

                        }, x => x.GetConstructors(), "Constructor")
                    }, x => entityNode.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.ExtendedBy)).Select(y => y.GetDestination()), "Concrete Decorator", GroupCheckType.Median)

                }, x => x.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.Implements) || y.GetRelationType().Equals(RelationType.Extends)).Select(y => y.GetDestination()), "Interface or parent class")

            }, x => new List<IEntityNode>() { entityNode }, "Decorator");
            var checkResult = decoratorCheck.Check(entityNode);

            result.Results = checkResult.GetChildFeedback().ToList();
            return result;
        }
    }
}
