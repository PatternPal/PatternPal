using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.Checks;
using IDesign.Recognizers.Models.Checks.Members;
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

            var propertyChecks = new FieldCheck()
                .Modifiers(Modifiers.Private.Not(), Modifiers.Static)
                .Type(entityNode);

            var constructorChecks = new MethodCheck()
                .Modifiers(Modifiers.Private.Not());


            var singletonCheck = new GroupCheck<IEntity, IEntity>(
                new List<ICheck<IEntity>>
                {
                    new GroupCheck<IEntity, IMethod>(methodChecks.ToList(), x => x.GetAllMethods(), "SingletonMethod"),
                    new GroupCheck<IEntity, IField>(
                        propertyChecks.ToList(), x => x.GetFields(),
                        "SingletonField"
                    ),
                    new GroupCheck<IEntity, IMethod>(
                        constructorChecks.ToList(), x => x.GetConstructors(),
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
