using System;
using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.Output;

namespace IDesign.Recognizers.Models.ElementChecks
{
    public enum GroupCheckType
    {
        Any,
        All
    }
    /// <summary>
    ///     A class for defining a check on a type with a predicate
    /// </summary>
    /// <typeparam name="T">The type witch the check is for</typeparam>
    public class GroupCheck<T> : ICheck<T> where T : ICheckable
    {
        private readonly List<ICheck<T>> _checks;
        private readonly string _description;
        private readonly Func<T, IEnumerable<T>> _elements;

        public GroupCheck(List<ICheck<T>> checks, Func<T, IEnumerable<T>> elements, string description)
        {
            _checks = checks;
            _description = description;
            _elements = elements;
        }

        public IFeedback Check(T elementToCheck)
        {
            var allChildFeedback = new Dictionary<T, (int score, IEnumerable<IFeedback> childFeedback)>();


            foreach (var check in _checks)
            {
                var elements = _elements(elementToCheck);
                var childFeedback = elements.Select(x => check.Check(x));
                
                

                var score = childFeedback.Sum(x => x.GetScore());
                allChildFeedback.Add(element, (score, childFeedback));
            }

            var childFeedbackList = new List<Feedback>();

            //Add feedback for best scored element
            foreach (var elementScore in allChildFeedback)
                //If element has the highest score
                if (elementScore.Value.score == allChildFeedback.Values.Select(x => x.score).Max())
                {
                    foreach (var feedback in elementScore.Value.childFeedback)
                        childFeedbackList.Add(
                            new Feedback(
                                _description,
                                FeedbackType.Correct,
                                elementScore.Key.GetSuggestionNode()
                            )
                        );

                    return (, elementScore.Value.score);
                }

            return new Feedback(_description, feedback, elementToCheck.GetSuggestionNode());
        }
    }
}