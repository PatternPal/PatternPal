
using IDesign.Checks;
using IDesign.Models;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Output;
using System.Collections.Generic;

namespace IDesign.Recognizers
{
    public class AdapterRecognizer : Recognizer, IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();
            var methodChecks = new List<ElementCheck<IMethod>>()
            {
                new ElementCheck<IMethod>(x => x.CheckReturnType(entityNode.GetName()) , "Incorrecte return type"),
                new ElementCheck<IMethod>(x => x.CheckModifier("static") , "Is niet static"),
                new ElementCheck<IMethod>(x => x.CheckReturnTypeSameAsCreation(), "Return type is niet hetzelfde als wat er gemaakt wordt" )
            };
            CheckElements(result, entityNode.GetMethods(), methodChecks);

            var propertyChecks = new List<ElementCheck<IField>>()
            {
               new ElementCheck<IField>(x => x.CheckFieldType(entityNode.GetName()) , "Incorrecte type"),
               new ElementCheck<IField>(x => x.CheckMemberModifier("static") , "Is niet static"),
               new ElementCheck<IField>(x => x.CheckMemberModifier("private") , "Is niet private")
            };
            CheckElements(result, entityNode.GetFields(), propertyChecks);

            var constructorChecks = new List<ElementCheck<IMethod>>()
            {
                 new ElementCheck<IMethod>(x => !x.CheckModifier("public") , "Is public moet private of protected zijn")
            };
            CheckElements(result, entityNode.GetConstructors(), constructorChecks);

            result.Score = (int)(result.Score / 7f * 100f);
            return result;
        }
    }
}
