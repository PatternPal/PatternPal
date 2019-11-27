using System;
using System.Collections.Generic;
using System.Text;
using IDesign.Checks;
using IDesign.Models;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Output;

namespace IDesign.Recognizers
{
    class FactoryRecognizer : Recognizer, IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();
            var methodChecks = new List<ElementCheck<IMethod>>()
            {
                new ElementCheck<IMethod>(x => x.CheckModifier("private") , "Is niet private"),
                new ElementCheck<IMethod>(x => x.CheckReturnTypeSameAsCreation(), "Return type is niet hetzelfde als wat er gemaakt wordt" )
            };
            CheckElements(result, entityNode.GetMethods(), x => x.GetName(), methodChecks);

           

            result.Score = (int)(result.Score / 2f * 100f);
            return result;
        }
    }
}
