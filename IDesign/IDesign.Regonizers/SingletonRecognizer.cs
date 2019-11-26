using IDesign.Checks;
using IDesign.Models;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Output;
using System.Collections.Generic;

namespace IDesign.Recognizers
{
    public class SingletonRecognizer : Recognizer, IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();
            var methodChecks = new List<ElementCheck<IMethod>>()
            {
                new ElementCheck<IMethod>(x => x.CheckReturnType(entityNode.GetName()) , "Incorrecte return type"),
                new ElementCheck<IMethod>(x => x.CheckModifier("static") , "Is niet static")
            };
            CheckElements(result, entityNode.GetMethods(), x => x.GetName(), methodChecks);

            var propertyChecks = new List<ElementCheck<IField>>()
            {
               new ElementCheck<IField>(x => x.CheckFieldType(entityNode.GetName()) , "Incorrecte type"),
               new ElementCheck<IField>(x => x.CheckMemberModifier("static") , "Is niet static"),
               new ElementCheck<IField>(x => x.CheckMemberModifier("private") , "Is niet private")
            };
            CheckElements(result, entityNode.GetFields(), x => x.GetName(), propertyChecks);

            result.Score = (int)(result.Score / 5f * 100f);

            return result;
        }
    }
}
