
﻿using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Output;
using System.Collections.Generic;

namespace IDesign.Recognizers
{
    public class SingletonRecognizer : Recognizer, IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();
            var methodChecks = new List<ElementCheck<IMethod>>
            {
                new ElementCheck<IMethod>(x => x.CheckReturnType(entityNode.GetName()) , "Incorrect return type"),
                new ElementCheck<IMethod>(x => x.CheckModifier("static") , "Is not static"),
                new ElementCheck<IMethod>(x => x.CheckReturnTypeSameAsCreation(), "Return type isnt the same as created" )
            };
            CheckElements(result, entityNode.GetMethods(), methodChecks);

            var propertyChecks = new List<ElementCheck<IField>>
            {
               new ElementCheck<IField>(x => x.CheckFieldType(entityNode.GetName()) , "Incorrect type"),
               new ElementCheck<IField>(x => x.CheckMemberModifier("static") , "Is not static"),
               new ElementCheck<IField>(x => x.CheckMemberModifier("private") , "Is not private")
            };
            CheckElements(result, entityNode.GetFields(), propertyChecks);

            var constructorChecks = new List<ElementCheck<IMethod>>
            {
                 new ElementCheck<IMethod>(x => !x.CheckModifier("public") , "Is public msut be private or protected")
            };
            CheckElements(result, entityNode.GetConstructors(), constructorChecks);

            result.Score = (int) (result.Score / 7f * 100f);
            return result;
        }
    }
}
