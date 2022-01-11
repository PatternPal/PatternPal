using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.Checks;
using IDesign.Recognizers.Models.Checks.Entities;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Models;

namespace IDesign.Recognizers.Recognizers
{
    public class SingletonRecognizer : IRecognizer
    {
        public IResult Recognize(IEntity entityNode)
        {
            var result = new EntityCheck()
                .Any.Method("SingletonMethod", true)
                    .Modifiers(Modifiers.Static, Modifiers.Private.Not())
                    .ReturnType(entityNode)
                    .Custom(
                        m => m.CheckReturnTypeSameAsCreation(),
                        new ResourceMessage("SingletonMethodReturnCreationType")
                    )
                .Any.Field("SingletonField", true)
                    .Modifiers(Modifiers.Public.Not(), Modifiers.Static)
                    .Type(entityNode)
                .Any.Constructor("SingletonConstructor")
                    .Modifiers(Modifiers.Public.Not())
                .Build()
                .ToResult(entityNode);


            result.GetRelatedSubTypes().Add(entityNode, "Singleton");
            return result;
        }
    }
}
