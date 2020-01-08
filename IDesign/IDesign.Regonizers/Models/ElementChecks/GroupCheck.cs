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
        private IResourceMessage _resourcemessage;
        private readonly Func<TParent, IEnumerable<TChild>> _elements;
        public GroupCheckType Type { get; set; }

        public GroupCheck(List<ICheck<TChild>> checks, Func<TParent, IEnumerable<TChild>> elements, string resourcemessage, GroupCheckType type = GroupCheckType.Any)
        {
            _checks = checks;
            _elements = elements;
            Type = type;
        }

        public GroupCheck(List<ICheck<TChild>> checks, Func<TParent, IEnumerable<TChild>> elements, IResourceMessage resourcemessage, GroupCheckType type = GroupCheckType.Any)
        {
            _checks = checks;
            _resourcemessage = resourcemessage;
            _elements = elements;
            Type = type;
        }

        public ICheckResult Check(TParent elementToCheck)
        {
            if (elementToCheck == null) return CreateFalseResult();

            var elements = _elements(elementToCheck).ToList();
            if (!elements.Any()) return CreateFalseResult();

            var allChildFeedback = new Dictionary<TChild, (float score, IEnumerable<ICheckResult> childFeedback)>();
            foreach (var element in elements)
            {
                var childFeedback = _checks.Select(x => x.Check(element));
                var score = childFeedback.Sum(x => x.GetScore());
                allChildFeedback.Add(element, (score, childFeedback));
            }

            if (Type == GroupCheckType.All)
                return CheckAll(elementToCheck, allChildFeedback);
            else if (Type == GroupCheckType.Any)
                return CheckAny(elementToCheck, allChildFeedback);
            else if (Type == GroupCheckType.Median)
                return CheckMedian(elementToCheck, allChildFeedback);
            return null;
        }

        private ICheckResult CheckAny(TParent elementToCheck, Dictionary<TChild, (float score, IEnumerable<ICheckResult> childFeedback)> allChildFeedback)
        {
            var highestScored = allChildFeedback.OrderByDescending(x => x.Value.score).FirstOrDefault();
            var feedback = FeedbackType.Incorrect;

            if (highestScored.Value.childFeedback.Any(x => x.GetFeedbackType() == FeedbackType.Correct))
                feedback = FeedbackType.SemiCorrect;
            if (highestScored.Value.childFeedback.All(x => x.GetFeedbackType() == FeedbackType.Correct))
                feedback = FeedbackType.Correct;

            return new CheckResult(_resourcemessage, feedback, elementToCheck)
            {
                ChildFeedback = highestScored.Value.childFeedback.ToList()
            };
        }

        private ICheckResult CheckAll(TParent elementToCheck, Dictionary<TChild, (float score, IEnumerable<ICheckResult> childFeedback)> allChildFeedback)
        {
            var feedback = FeedbackType.Correct;
            if (allChildFeedback.Values.All(x => x.score != 0))
                feedback = FeedbackType.Incorrect;
            if (allChildFeedback.Values.Any(x => x.score != 0))
                feedback = FeedbackType.SemiCorrect;

            var childResults = new List<ICheckResult>();
            foreach (var valueTuple in allChildFeedback)
            {
                childResults.Add(new CheckResult(_resourcemessage, feedback, elementToCheck)
                {
                    ChildFeedback = valueTuple.Value.childFeedback.ToList()
                });
            }

            return new CheckResult(_resourcemessage, feedback, elementToCheck)
            {
                ChildFeedback = childResults
            };
        }

        private ICheckResult CheckMedian(TParent elementToCheck, Dictionary<TChild, (float score, IEnumerable<ICheckResult> childFeedback)> allChildFeedback)
        {
            var feedback = FeedbackType.Correct;
            if (allChildFeedback.Values.All(x => x.score != 0))
                feedback = FeedbackType.Incorrect;
            if (allChildFeedback.Values.Any(x => x.score != 0))
                feedback = FeedbackType.SemiCorrect;

            var childResults = new List<ICheckResult>();
            foreach (var valueTuple in allChildFeedback)
            {
                var test = valueTuple.Value;

                childResults.Add(new CheckResult(_resourcemessage, feedback, elementToCheck)
                {
                    ChildFeedback = valueTuple.Value.childFeedback.ToList()
                });
            }

            ChangeScore(childResults, childResults.Sum(x => x.GetTotalChecks()), childResults.Count() * childResults.Count());

           
            return new CheckResult(_resourcemessage, feedback, elementToCheck)
            {
                ChildFeedback = childResults
            };
        }

        private void ChangeScore(IEnumerable<ICheckResult> results, float score, float count)
        {
            var newScore = score / count;
            results.ToList().ForEach(x =>
            {
                x.ChangeScore(score / count);
                ChangeScore(x.GetChildFeedback(), newScore, x.GetChildFeedback().Count());
            });
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