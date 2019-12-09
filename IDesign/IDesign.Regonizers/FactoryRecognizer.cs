using System.Collections.Generic;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;

namespace IDesign.Recognizers
{
    public class FactoryRecognizer :  IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();
            var methodChecks = new List<ElementCheck<IMethod>>
            {
                new ElementCheck<IMethod>(x => x.CheckModifier("public") , "Is not public"),
                new ElementCheck<IMethod>(x => x.CheckReturnTypeSameAsCreation(), "Return type isnt the same as created" )
            };
            //CheckElements(result, entityNode.GetMethods(), methodChecks);

            result.Score = (int)(result.Score / 2f * 100f);
            return result;
        }
    }
}
