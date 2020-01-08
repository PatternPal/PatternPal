using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;

namespace IDesign.Recognizers
{
    public class SingletonRecognizer : IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();

            var methodChecks = new List<ICheck<IMethod>>
            {
                new ElementCheck<IMethod>(x => x.CheckReturnType(entityNode.GetName()), "Incorrect return type",1),
                new ElementCheck<IMethod>(x => x.CheckModifier("static"), "Is not static",1),
                new ElementCheck<IMethod>(x => x.CheckReturnTypeSameAsCreation(),
                    "Return type isnt the same as created")
            };

            var propertyChecks = new List<ICheck<IField>>
            {
                new ElementCheck<IField>(x => x.CheckFieldType(new List<string>(){ entityNode.GetName() }), "Incorrect type",1),
                new ElementCheck<IField>(x => x.CheckMemberModifier("static"), "Is not static",1),
                new ElementCheck<IField>(x => x.CheckMemberModifier("private"), "Is not private",0.5f)
            };

            var constructorChecks = new List<ICheck<IMethod>>
            {
                new ElementCheck<IMethod>(x => (x.CheckModifier("private")) ||(x.CheckModifier("protected")), "should be private or protected",0.5f)
            };


            var singletonCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new GroupCheck<IEntityNode, IMethod>(methodChecks, x => x.GetMethods(), "Has GetInstance()"),
                new GroupCheck<IEntityNode, IField>(propertyChecks, x => x.GetFields(), "Has instance of itself"),
                new GroupCheck<IEntityNode, IMethod>(constructorChecks, x => x.GetConstructors(), "Private constructor")
            }, x => new List<IEntityNode> { entityNode }, "Singleton", GroupCheckType.All);


            var r = singletonCheck.Check(entityNode);

            result.Results = r.GetChildFeedback().ToList();
            return result;
        }
    }
}