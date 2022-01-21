using System;
using PatternPal.Recognizers.Abstractions;
using PatternPal.Recognizers.Models.Output;
using SyntaxTree.Abstractions;

namespace PatternPal.Recognizers.Models.ElementChecks
{
    /// <summary>
    ///     A class for defining a check on an element with a predicate
    /// </summary>
    /// <typeparam name="T">Type of the element to be checked</typeparam>
    public class ElementCheck<T> : ICheck<T> where T : INode
    {
        private readonly IResourceMessage _feedback;
        private readonly Predicate<T> _predicate;
        private readonly CheckType _checkType;

        private readonly float _score;

        public ElementCheck(Predicate<T> predicate, string description, float score = 1, CheckType checkType = CheckType.Optional)
        {
            _predicate = predicate;
            _feedback = new ResourceMessage(description);
            _score = score;
            _checkType = checkType;
        }

        public ElementCheck(Predicate<T> predicate, IResourceMessage feedback, float score = 1, CheckType checkType = CheckType.Optional)
        {
            _predicate = predicate;
            _feedback = feedback;
            _score = score;
            _checkType = checkType;
        }

        public ElementCheck(Predicate<T> predicate, string description, CheckType checkType) : this(predicate, description, 1, checkType) { }

        public ElementCheck(Predicate<T> predicate, IResourceMessage feedback, CheckType checkType) : this(predicate, feedback, 1, checkType) { }

        public ICheckResult Check(T elementToCheck)
        {
            //Support checking without input (To create false result)
            if (elementToCheck != null)
            {
                var isValid = _predicate(elementToCheck);
                var feedback = isValid ? FeedbackType.Correct : FeedbackType.Incorrect;
                var checkResult = new CheckResult(_feedback, feedback, elementToCheck, _score);

                if (feedback == FeedbackType.Incorrect && _checkType == CheckType.KnockOut)
                {
                    checkResult.HasIncorrectKnockOutCheck = true;
                }

                return checkResult;
            }

            return new CheckResult(_feedback, FeedbackType.Incorrect, null, _score);
        }
    }
}
