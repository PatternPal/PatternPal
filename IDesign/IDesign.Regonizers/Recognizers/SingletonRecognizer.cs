using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;
using SyntaxTree.Models;

namespace IDesign.Recognizers.Recognizers
{
    public class SingletonRecognizer : IRecognizer
    {
        public IResult Recognize(IEntity entityNode)
        {
            var result = new Result();

            var methodChecks = new MethodCheck()
                .Modifiers(Modifiers.Static, Modifiers.Private.Not())
                .ReturnType(entityNode)
                .Custom(
                    m => m.CheckReturnTypeSameAsCreation(),
                    new ResourceMessage("SingletonMethodReturnCreationType")
                );

            var propertyChecks = new List<ICheck<IField>>
            {
                new ElementCheck<IField>(
                    x => x.CheckFieldTypeGeneric(new List<string> { entityNode.GetName() }),
                    new ResourceMessage("FieldType", entityNode.GetName()), 1
                ),
                new ModifierCheck(Modifiers.Private.Not(), Modifiers.Static),
            };

            var constructorChecks = new List<ICheck<IMethod>>
            {
                new ElementCheck<IMethod>(x => !x.CheckModifier("public"), "ConstructorModifierNotPublic", 0.5F)
            };


            var singletonCheck = new GroupCheck<IEntity, IEntity>(
                new List<ICheck<IEntity>>
                {
                    new GroupCheck<IEntity, IMethod>(methodChecks.ToList(), x => x.GetAllMethods(), "SingletonMethod"),
                    new GroupCheck<IEntity, IField>(
                        propertyChecks, x => x.GetFields(),
                        "SingletonField"
                    ),
                    new GroupCheck<IEntity, IMethod>(
                        constructorChecks, x => x.GetConstructors(),
                        "SingletonConstructor"
                    )
                }, x => new List<IEntity> { entityNode }, "Singleton", GroupCheckType.All
            );

            var r = singletonCheck.Check(entityNode);
            result.Results = r.GetChildFeedback().ToList();


            result.RelatedSubTypes.Add(entityNode, "Singleton");
            return result;
        }
    }
}
