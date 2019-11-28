using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Output;
using IDesign.Recognizers.Checks;

namespace IDesign.Recognizers
{
    class FactoryRecognizer : Recognizer, IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();
            var methodChecks = new List<ElementCheck<IMethod>>()
            {
                new ElementCheck<IMethod>(x => CreatedClassImplementsReturnTypeInterface(entityNode, x) ,
                "Return type is niet hetzelfde als wat er gemaakt wordt" )
            };
            CheckElements(result, entityNode.GetMethods(), x => x.GetName(), methodChecks);

            result.Score = (int)(result.Score / 2f * 100f);
            return result;
        }

        private bool CreatedClassImplementsReturnTypeInterface(IEntityNode node, IMethod method)
        {
           return method.GetCreatedTypes()
                .Where(name => node.GetEdgeNode(name).ImplementsInterface(method.GetReturnType())).Any()
        }
    }
}
