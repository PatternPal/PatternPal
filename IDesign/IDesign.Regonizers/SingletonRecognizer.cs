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
                new ElementCheck<IMethod>(x => x.CheckReturnType(entityNode.GetName()), new ResourceMessage("MethodReturnType", new [] { entityNode.GetName() } )),
                new ElementCheck<IMethod>(x => x.CheckModifier("static"), new ResourceMessage("MethodModifierStatic")),
                new ElementCheck<IMethod>(x => x.CheckReturnTypeSameAsCreation(),
                   new ResourceMessage("SingletonMethodReturnCreationType"))
            };

            var propertyChecks = new List<ICheck<IField>>
            {
                new ElementCheck<IField>(x => x.CheckFieldType(new List<string>(){ entityNode.GetName() }), new ResourceMessage("FieldType", new []{ entityNode.GetName()})),
                new ElementCheck<IField>(x => x.CheckMemberModifier("static"), new ResourceMessage("FieldModifierStatic")),
                new ElementCheck<IField>(x => !x.CheckMemberModifier("public"), new ResourceMessage("FieldModifierNotPublic"))
            };

            var constructorChecks = new List<ICheck<IMethod>>
            {
                new ElementCheck<IMethod>(x => !x.CheckModifier("public"), new ResourceMessage("ConstructorModifierNotPublic"))
            };


            var singletonCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new GroupCheck<IEntityNode, IMethod>(methodChecks, x => x.GetMethods(), new ResourceMessage("SingletonMethod")),
                new GroupCheck<IEntityNode, IField>(propertyChecks, x => x.GetFields(), new ResourceMessage("SingletonField")),
                new GroupCheck<IEntityNode, IMethod>(constructorChecks, x => x.GetConstructors(), new ResourceMessage("SingletonConstructor"))
            }, x => new List<IEntityNode> {entityNode}, "Singleton", GroupCheckType.All);


            var r = singletonCheck.Check(entityNode);

            result.Results = r.GetChildFeedback().ToList();
            return result;
        }
    }
}