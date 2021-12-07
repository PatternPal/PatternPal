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
        All,
        Median
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
        private readonly Func<TParent, IEnumerable<TChild>> _elements;
        private readonly IResourceMessage _resourcemessage;

        public GroupCheck(
            List<ICheck<TChild>> checks,
            Func<TParent, IEnumerable<TChild>> elements,
            string resourcemessage,
            GroupCheckType type = GroupCheckType.Any
        )
        {
            _checks = checks;
            _resourcemessage = new ResourceMessage(resourcemessage);
            _elements = elements;
            Type = type;
        }

        public GroupCheck(
            List<ICheck<TChild>> checks,
            Func<TParent, IEnumerable<TChild>> elements,
            IResourceMessage resourcemessage,
            GroupCheckType type = GroupCheckType.Any
        )
        {
            _checks = checks;
            _resourcemessage = resourcemessage;
            _elements = elements;
            Type = type;
        }

        public GroupCheckType Type { get; set; }

        public ICheckResult Check(TParent elementToCheck)
        {
            if (elementToCheck == null)
            {
                return CreateFalseResult();
            }

            var elements = _elements(elementToCheck).ToList();

            if (!elements.Any())
            {
                return CreateFalseResult();
            }

            var allChildFeedback = new Dictionary<TChild, (float score, IEnumerable<ICheckResult> childFeedback)>();
            foreach (var (element, childFeedback, score) in from element in elements
                let childFeedback = _checks.Select(x => x.Check(element))
                let score = childFeedback.Sum(x => x.GetScore())
                select (element, childFeedback, score))
            {
                allChildFeedback.Add(element, (score, childFeedback));
            }

            switch (Type)
            {
                case GroupCheckType.All: return CheckAll(elementToCheck, allChildFeedback);
                case GroupCheckType.Any: return CheckAny(elementToCheck, allChildFeedback);
                case GroupCheckType.Median: return CheckMedian(elementToCheck, allChildFeedback);
            }

            return null;
        }

        private ICheckResult CheckAny(
            TParent elementToCheck,
            Dictionary<TChild,
                (float score, IEnumerable<ICheckResult> childFeedback)> allChildFeedback
        )
        {
            var highestScored = allChildFeedback.OrderByDescending(x => x.Value.score).FirstOrDefault();
            var feedback = FeedbackType.Incorrect;

            if (highestScored.Value.childFeedback.Any(x => x.GetFeedbackType() == FeedbackType.Correct))
            {
                feedback = FeedbackType.SemiCorrect;
            }

            if (highestScored.Value.childFeedback.All(x => x.GetFeedbackType() == FeedbackType.Correct))
            {
                feedback = FeedbackType.Correct;
            }

            return new CheckResult(_resourcemessage, feedback, elementToCheck)
            {
                ChildFeedback = highestScored.Value.childFeedback.ToList()
            };
        }

        private ICheckResult CheckAll(
            TParent elementToCheck,
            Dictionary<TChild,
                (float score, IEnumerable<ICheckResult> childFeedback)> allChildFeedback
        )
        {
            var feedback = FeedbackType.Correct;
            if (allChildFeedback.Values.All(x => x.score != 0))
            {
                feedback = FeedbackType.Incorrect;
            }

            if (allChildFeedback.Values.Any(x => x.score != 0))
            {
                feedback = FeedbackType.SemiCorrect;
            }

            var childResults = new List<ICheckResult>();
            foreach (var valueTuple in allChildFeedback)
            {
                childResults.Add(
                    new CheckResult(valueTuple.Key.GetSuggestionName(), feedback, elementToCheck)
                    {
                        ChildFeedback = valueTuple.Value.childFeedback.ToList()
                    }
                );
            }

            return new CheckResult(_resourcemessage, feedback, elementToCheck) { ChildFeedback = childResults };
        }

        private ICheckResult CheckMedian(
            TParent elementToCheck,
            Dictionary<TChild, (float score, IEnumerable<ICheckResult> childFeedback)> allChildFeedback
        )
        {
            var feedback = FeedbackType.Correct;
            if (allChildFeedback.Values.All(x => x.score != 0))
            {
                feedback = FeedbackType.Incorrect;
            }

            if (allChildFeedback.Values.Any(x => x.score != 0))
            {
                feedback = FeedbackType.SemiCorrect;
            }

            var childResults = new List<ICheckResult>();
            foreach (var valueTuple in allChildFeedback)
            {
                var (score, childFeedback) = valueTuple.Value;

                childResults.Add(
                    new CheckResult(valueTuple.Key.GetSuggestionName(), feedback, valueTuple.Key)
                    {
                        _feedback = _resourcemessage, ChildFeedback = valueTuple.Value.childFeedback.ToList()
                    }
                );
            }

            return new CheckResult(_resourcemessage, feedback, elementToCheck)
            {
                ChildFeedback = childResults, CalculationType = CheckCalculationType.Average
            };
        }

        private ICheckResult CreateFalseResult()
        {
            return new CheckResult(_resourcemessage, FeedbackType.Incorrect, null)
            {
                ChildFeedback = _checks.Select(x => x.Check(null)).ToList()
            };
        }
    }
}
