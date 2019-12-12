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
    ///     A class for defining checks for the children of a parent class
    /// </summary>
    /// <typeparam name="TParent">Type of the element with sub elements</typeparam>
    /// <typeparam name="TChild">Type of the elements that are checked</typeparam>
    public class GroupCheck<TParent, TChild> : ICheck<TParent>
        where TParent : class, ICheckable where TChild : class, ICheckable
    {
        private readonly List<ICheck<TChild>> _checks;
        private readonly string _description;
        private readonly Func<TParent, IEnumerable<TChild>> _elements;
        public GroupCheckType _type { get; set; }

        public GroupCheck(List<ICheck<TChild>> checks, Func<TParent, IEnumerable<TChild>> elements, string description, GroupCheckType type = GroupCheckType.Any)
        {
            _checks = checks;
            _description = description;
            _elements = elements;
            _type = type;
        }

        public ICheckResult Check(TParent elementToCheck)
        {
            if (elementToCheck == null) return CreateFalseResult();

            var elements = _elements(elementToCheck).ToList();
            if (!elements.Any()) return CreateFalseResult();

            var allChildFeedback = new Dictionary<TChild, (int score, IEnumerable<ICheckResult> childFeedback)>();
            foreach (var element in elements)
            {
                var childFeedback = _checks.Select(x => x.Check(element));
                var score = childFeedback.Sum(x => x.GetScore());
                allChildFeedback.Add(element, (score, childFeedback));
            }

            if (_type == GroupCheckType.All)
                return CheckAll(elementToCheck, allChildFeedback);
            else if (_type == GroupCheckType.Any)
                return CheckAny(elementToCheck, allChildFeedback);
            return null;
        }

        private ICheckResult CheckAny(TParent elementToCheck, Dictionary<TChild, (int score, IEnumerable<ICheckResult> childFeedback)>  allChildFeedback)
        {
            var highestScored = allChildFeedback.OrderByDescending(x => x.Value.score).FirstOrDefault();
            var feedback = FeedbackType.Incorrect;

            if (highestScored.Value.childFeedback.Any(x => x.GetFeedbackType() == FeedbackType.Correct))
                feedback = FeedbackType.SemiCorrect;
            if (highestScored.Value.childFeedback.All(x => x.GetFeedbackType() == FeedbackType.Correct))
                feedback = FeedbackType.Correct;

            var message = elementToCheck.GetSuggestionName() + " | " + _description;
            return new CheckResult(message, feedback, elementToCheck.GetSuggestionNode())
            {
                ChildFeedback = highestScored.Value.childFeedback.ToList()
            };
        }
        private ICheckResult CheckAll(TParent elementToCheck, Dictionary<TChild, (int score, IEnumerable<ICheckResult> childFeedback)> allChildFeedback)
        {
            var feedback = FeedbackType.Correct;
            if (allChildFeedback.Values.All(x => x.score != 0))
                feedback = FeedbackType.Incorrect;
            if (allChildFeedback.Values.Any(x => x.score != 0))
                feedback = FeedbackType.SemiCorrect;

            var childResults = new List<ICheckResult>();
            foreach (var valueTuple in allChildFeedback)
            {
                childResults.Add(new CheckResult(_description, feedback, elementToCheck.GetSuggestionNode())
                {
                    ChildFeedback = valueTuple.Value.childFeedback.ToList()
                });
            }

            var message = elementToCheck.GetSuggestionName() + " | " + _description;
            return new CheckResult(message, feedback, elementToCheck.GetSuggestionNode())
            {
                ChildFeedback = childResults
            };
        }

        private ICheckResult CreateFalseResult()
        {
            return new CheckResult(_description, FeedbackType.Incorrect, null)
            {
                ChildFeedback = _checks.Select(x => x.Check(null)).ToList()
            };
        }
    }
}