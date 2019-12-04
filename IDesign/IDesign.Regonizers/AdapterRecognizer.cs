
using IDesign.Checks;
using IDesign.Models;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Output;
using System.Collections.Generic;
using System.Linq;

namespace IDesign.Recognizers
{
    public class AdapterRecognizer : Recognizer, IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();
            // Adapter
            // Implements interface
            if (entityNode.GetRelations().Any(x => x.GetRelationType() == RelationType.Implements))
                result.Score += 1;

            // Has field with class (adapter)
            // As many public functions as adapter
            // All functions uses the adapter

            //result.Score = (int)(result.Score / 7f * 100f);
            entityNode.GetTy


            return result;
        }

        


    }
}
