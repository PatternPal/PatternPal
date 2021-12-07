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
        private readonly IResourceMessage _feedback;
        private readonly Predicate<T> _predicate;

        private readonly float _score;

        public ElementCheck(Predicate<T> predicate, string description)
        {
            _predicate = predicate;
            _feedback = new ResourceMessage(description);
            _score = 1;
        }

        public ElementCheck(Predicate<T> predicate, string description, float score)
        {
            _predicate = predicate;
            _feedback = new ResourceMessage(description);
            _score = score;
        }

        public ElementCheck(Predicate<T> predicate, IResourceMessage feedback)
        {
            _predicate = predicate;
            _feedback = feedback;
            _score = 1;
        }

        public ElementCheck(Predicate<T> predicate, IResourceMessage feedback, float score)
        {
            _predicate = predicate;
            _feedback = feedback;
            _score = score;
        }

        public ICheckResult Check(T elementToCheck)
        {
            //Support checking without input (To create false result)
            if (elementToCheck != null)
            {
                var isValid = _predicate(elementToCheck);
                var feedback = isValid ? FeedbackType.Correct : FeedbackType.Incorrect;
                return new CheckResult(_feedback, feedback, elementToCheck, _score);
            }

            return new CheckResult(_feedback, FeedbackType.Incorrect, null, _score);
        }
    }
}
