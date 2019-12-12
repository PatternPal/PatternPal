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
                new ElementCheck<IEntityNode>(x => x.CheckModifier("Abstract"), "De class moet abstract zijn"),
                new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.Extends, 1) || x.CheckMinimalAmountOfRelationTypes(RelationType.Implements, 1), "De class zit niet onder een interface of parent class"),
                new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.ExtendedBy, 1), "De class wordt niet extend door een ander class"),
                
                //Component checks
                new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>{
                    new ElementCheck<IEntityNode>(x => {currentComponent = x; return x.GetMethods().Any(); }, "Heeft functies"),
                    new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.ExtendedBy, 2) || x.CheckMinimalAmountOfRelationTypes(RelationType.ImplementedBy, 2), "De class wordt niet extended of geimplementeerd door een andere class"),

                    new GroupCheck<IEntityNode, IMethod>( new List<ICheck<IMethod>>{
                        new ElementCheck<IMethod>(x => x.CheckModifier("public") || x.CheckModifier("protected") , "De constructor moet public of protected zijn"),
                        new ElementCheck<IMethod>(x => x.CheckParameters(new List<string>{currentComponent.GetName() }), "De constructor moet de interface of parent als parameter hebben")

                    }, x => entityNode.GetConstructors(), "Constructor heeft dit bla bla "),
                    new GroupCheck<IEntityNode, IField>( new List<ICheck<IField>>{
                        new ElementCheck<IField>(x => x.CheckFieldType(new List<string>{currentComponent.GetName() }) , "Incorrect type")

                    }, x => entityNode.GetFields(), "Field heeft dit bla bla "),

                    //Concrete decorator
                    new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>{
                        new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.CheckModifier("public"), "Constructor moet public of protected zijn"),
                            new ElementCheck<IMethod>(x => x.CheckParameters(new List<string>() { currentComponent.GetName() }), "De class moet de component als parameter in de constructor hebben"),
                            new ElementCheck<IMethod>(x => x.CheckArguments(currentComponent.GetName()), "De class moet de component als argument in de constructor hebben")

                        }, x => x.GetConstructors(), "")
                    }, x => entityNode.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.ExtendedBy)).Select(y => y.GetDestination()), "", GroupCheckType.All)

                }, x => x.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.Implements) || y.GetRelationType().Equals(RelationType.Extends)).Select(y => y.GetDestination()), "")

                   }, x => new List<IEntityNode>() { entityNode }, "Klasse checks");
            var checkResult = decoratorCheck.Check(entityNode);

            result.Results = checkResult.GetChildFeedback().ToList();
            return result;
        }
    }
}
