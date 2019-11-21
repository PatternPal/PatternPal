using IDesign.Checks;
using IDesign.Regonizers.Abstractions;
using IDesign.Regonizers.Output;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IDesign.Regonizers
{
    public class SingletonRecognizer : IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();
            var methodScores = new Dictionary<MethodDeclarationSyntax, (int score, IList<string> suggestions)>();

            //Give scores to methods
            foreach (var method in entityNode.GetMethods())
            {
                var correctReturnType = method.CheckReturnType(entityNode.GetName());
                var correctModifier = method.CheckMemberModifier("static");

                var score = correctReturnType ? 1 : 0;
                score += correctModifier ? 1 : 0;

                var suggestions = new List<string>();

                if (!correctReturnType) suggestions.Add("Incorrecte return type");
                if (!correctModifier) suggestions.Add("Is niet static");

                methodScores.Add(method, (score, suggestions));
            }

            //Add suggestions for best scored method
            foreach (var methodResult in methodScores)
            {
                if (methodResult.Value.score == methodScores.Values.Select(x => x.score).Max())
                {
                    result.Score += methodResult.Value.score;
                    foreach (var methodSuggestion in methodResult.Value.suggestions)
                    {
                        result.Suggestions.Add(new Suggestion(methodResult.Key.Identifier.ToString() + ": " + methodSuggestion));
                    }
                    break;
                }
            }

            var propertyScores = new Dictionary<PropertyDeclarationSyntax, (int score, IList<string> suggestions)>();

            //Give scores to properies
            foreach (var property in entityNode.GetProperties())
            {
                var correctType = property.CheckPropertyType(entityNode.GetName());
                var isStatic = property.CheckMemberModifier("static");
                var isPrivate = property.CheckMemberModifier("private");

                var score = isPrivate ? 1 : 0;
                score += isStatic ? 1 : 0;
                score += correctType ? 1 : 0;

                var suggestions = new List<string>();

                if (!correctType) suggestions.Add("Incorrecte return type");
                if (!isStatic) suggestions.Add("Is niet static");
                if (!isPrivate) suggestions.Add("Is niet private");

                propertyScores.Add(property, (score, suggestions));
            }

            //Add suggestions for best scored method
            foreach (var methodResult in propertyScores)
            {
                if (methodResult.Value.score == propertyScores.Values.Select(x => x.score).Max())
                {
                    result.Score += methodResult.Value.score;
                    foreach(var propertySuggestion in methodResult.Value.suggestions)
                    {
                        result.Suggestions.Add(new Suggestion(methodResult.Key.Identifier.ToString() + ": " + propertySuggestion));
                    }
                    break;
                }
            }


            result.Score = (int)(result.Score / 5f * 100f);

            return result;
        }
    }
}
