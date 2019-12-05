using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models;
using IDesign.Recognizers.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Recognizers
{
    public class StateRecognizer : Recognizer, IRecognizer
    {
        public IResult Recognize(IEntityNode node)
        {
            var result = new Result();

            if (node.GetEntityNodeType() == EntityNodeType.Class)
            {
                Console.WriteLine("class");
                //var classChecks = new List<ElementCheck<IEntityNode>>
                //{
                //     new ElementCheck<IEntityNode>(x => x.CheckModifier("abstract"), "Incorrect return type")
                //};
            }
            if (node.GetEntityNodeType() == EntityNodeType.Interface)
            {
                Console.WriteLine("interface");
            }


            result.Score = (int)(result.Score / 7f * 100f);
            return result;
        }
    }
}
