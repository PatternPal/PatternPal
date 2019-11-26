using System;
using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Output;

namespace IDesign.Recognizers
{
    public partial class Recognizer
    {
        internal void CheckElements<T>(Result result, IEnumerable<T> elements, Func<T, string> elementName,
            IEnumerable<ElementCheck<T>> checks)
        {
            var checkResult = CheckElements(elements, elementName, checks);
            result.Score += checkResult.score;
            result.Suggestions.AddRange(checkResult.suggestions);
        }

        internal (IList<ISuggestion> suggestions, int score) CheckElements<T>(IEnumerable<T> elements,
            Func<T, string> elementName, IEnumerable<ElementCheck<T>> checks)
        {
            var suggestionList = new List<ISuggestion>();

            var scores = new Dictionary<T, (int score, IList<string> suggestions)>();

            //Give scores to elements
            foreach (var element in elements)
            {
                var score = 0;
                var suggestions = new List<string>();

                foreach (var check in checks)
                {
                    var isValid = check.Check(element);
                    score += isValid ? 1 : 0;

                    if (!isValid) suggestions.Add(check.GetSuggestionMessage());
                }

                scores.Add(element, (score, suggestions));
            }

            //Add suggestions for best scored element
            foreach (var elementScore in scores)
                //If element has the highest score
                if (elementScore.Value.score == scores.Values.Select(x => x.score).Max())
                {
                    foreach (var propertySuggestion in elementScore.Value.suggestions)
                        suggestionList.Add(new Suggestion(elementName(elementScore.Key) + ": " + propertySuggestion));

                    return (suggestionList, elementScore.Value.score);
                }

            return (suggestionList, 0);
        }
    }
}