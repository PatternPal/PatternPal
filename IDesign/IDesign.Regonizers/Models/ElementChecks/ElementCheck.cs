using System;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.Output;

namespace IDesign.Recognizers.Models.ElementChecks
{
    /// <summary>
    ///     A class for defining a check on an element with a predicate
    /// </summary>
    /// <typeparam name="T">Type of the element to be checked</typeparam>
    public class ElementCheck<T> : ICheck<T> where T : class, ICheckable
    {
        private readonly string _description;
        private readonly Predicate<T> _predicate;
        private readonly float _score;

        public ElementCheck(Predicate<T> predicate, string description)
        {
            _predicate = predicate;
            _description = description;
            _score = 1;
        }

        public ElementCheck(Predicate<T> predicate, string description, float score)
        {
            _predicate = predicate;
            _description = description;
            _score = score;
        }

        public ICheckResult Check(T elementToCheck)
        {
            //Support checking without input (To create false result)
            if (elementToCheck == null)
                return new CheckResult(_description, FeedbackType.Incorrect, null, _score);
            var isValid = _predicate(elementToCheck);
            var feedback = isValid ? FeedbackType.Correct : FeedbackType.Incorrect;
            var message = elementToCheck.GetSuggestionName() + " | " + _description;
            return new CheckResult(message, feedback, elementToCheck.GetSuggestionNode(), _score);
        }

        public string GetDescription()
        {
            return _description;
        }
    }
}