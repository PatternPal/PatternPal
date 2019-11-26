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
            var constructors = entityNode.GetConstructors();
            {
                new ElementCheck<IMethod>(x => x.CheckReturnType(entityNode.GetName()) , "Incorrecte return type"),
                new ElementCheck<IMethod>(x => x.CheckModifier("static") , "Is niet static")
                (x => x.CheckReturnTypeSameAsCreation(), "Return type is niet hetzelfde als wat er gemaakt wordt" )
            };
            CheckElements(result, entityNode.GetMethods(), x => x.GetName(), methodChecks);

            var propertyChecks = new List<ElementCheck<IField>>()
            {
               new ElementCheck<IField>(x => x.CheckFieldType(entityNode.GetName()) , "Incorrecte type"),
               new ElementCheck<IField>(x => x.CheckMemberModifier("static") , "Is niet static"),
               new ElementCheck<IField>(x => x.CheckMemberModifier("private") , "Is niet private")
            };
            CheckElements(result, entityNode.GetFields(), x => x.GetName(), propertyChecks);

            var constructorChecks = new List<(Predicate<IMethod> check, string suggestionMessage)>()
            {
                (x => !x.CheckModifier("public") , "Is public moet private of protected zijn")
            };
            CheckElements(result, entityNode.GetConstructors(), x => x.GetName(), constructorChecks);



            result.Score = (int)(result.Score / 7f * 100f);

            return result;
        }
    }
}
