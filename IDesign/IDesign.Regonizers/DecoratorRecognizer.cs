using IDesign.Checks;
using IDesign.Models;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Output;
using System.Collections.Generic;

namespace IDesign.Recognizers
{
    public class DecoratorRecognizer : Recognizer, IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();
            var constructorChecks = new List<ElementCheck<IMethod>>()
            {
                 new ElementCheck<IMethod>(x => x.CheckModifier("public") || x.CheckModifier("protected") , "Constructor moet public of protected zijn"),
                 new ElementCheck<IMethod>(x => x.CheckParameters(new List<string>(){ "string", "int" }), "Jeanrisotto")
            };
            CheckElements(result, entityNode.GetConstructors(), constructorChecks);

            result.Score = (int)(result.Score / 2f * 100f);
            return result;
        }
    }
}
