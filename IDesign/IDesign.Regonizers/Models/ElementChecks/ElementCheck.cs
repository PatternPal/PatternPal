using System;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.Output;

namespace IDesign.Recognizers.Models.ElementChecks
{
    /// <summary>
    ///     A class for defining a check on a type with a predicate
    /// </summary>
    /// <typeparam name="T">The type witch the check is for</typeparam>
    public class ElementCheck<T> : ICheck<T> where T : ICheckable
    {
        private readonly string _description;
        private readonly Predicate<T> _predicate;

        public ElementCheck(Predicate<T> predicate, string description)
        {
            _predicate = predicate;
            _description = description;
        }

        public string GetDescription()
        {
            return _description;
        }

        public IFeedback Check(T elementToCheck)
        {
            var isValid = _predicate(elementToCheck);
            var feedback = isValid ? FeedbackType.Correct : FeedbackType.Incorrect;
            return new Feedback(_description, feedback, elementToCheck.GetSuggestionNode());
        }
    }
}