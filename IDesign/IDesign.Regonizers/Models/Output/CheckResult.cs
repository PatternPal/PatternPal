using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using SyntaxTree.Abstractions;

namespace IDesign.Recognizers.Models.Output
{
    public enum CheckCalculationType
    {
        Sum,
        Average
    }

    public class CheckResult : ICheckResult
    {
        public CheckResult(string message, FeedbackType feedbackType, INode element)
        {
            Message = message;
            FeedbackType = feedbackType;
            Element = element;
        }

        public CheckResult(IResourceMessage feedback, FeedbackType feedbackType, INode element)
        {
            _feedback = feedback;
            FeedbackType = feedbackType;
            Element = element;
        }

        public CheckResult(IResourceMessage feedback, FeedbackType feedbackType, INode element, float score)
        {
            _feedback = feedback;
            FeedbackType = feedbackType;
            Element = element;
            Score = score;
        }

        public CheckResult(string message, FeedbackType feedbackType, INode element, float score)
        {
            Message = message;
            FeedbackType = feedbackType;
            Element = element;
            Score = score;
        }

        public CheckCalculationType CalculationType { get; set; } = CheckCalculationType.Sum;

        public string Message { get; set; }
        public FeedbackType FeedbackType { get; set; }
        public INode Element { get; set; }
        public List<ICheckResult> ChildFeedback { get; set; } = new List<ICheckResult>();
        public IResourceMessage _feedback { get; set; }
        public float Score { get; set; }

        public IEnumerable<ICheckResult> GetChildFeedback()
        {
            return ChildFeedback;
        }

        public float GetScore()
        {
            return !ChildFeedback.Any()
                ? FeedbackType == FeedbackType.Correct ? Score : 0
                : CalculationType == CheckCalculationType.Average
                    ? ChildFeedback.Average(x => x.GetScore())
                    : ChildFeedback.Sum(x => x.GetScore());
        }

        public float GetTotalChecks()
        {
            return ChildFeedback.Count == 0
                ? Score
                : CalculationType == CheckCalculationType.Average
                    ? ChildFeedback.Average(x => x.GetTotalChecks())
                    : ChildFeedback.Sum(x => x.GetTotalChecks());
        }

        public FeedbackType GetFeedbackType()
        {
            if (ChildFeedback.Count == 0)
            {
                return FeedbackType;
            }

            var feedback = FeedbackType.Incorrect;

            if (ChildFeedback.All(x => x.GetFeedbackType() == FeedbackType.Correct))
            {
                feedback = FeedbackType.Correct;
            }
            else if (ChildFeedback.Any(x => x.GetFeedbackType() == FeedbackType.Correct))
            {
                feedback = FeedbackType.SemiCorrect;
            }

            return feedback;
        }

        public INode GetElement()
        {
            return Element;
        }

        public IResourceMessage GetFeedback()
        {
            return _feedback;
        }

        public void ChangeScore(float score)
        {
            Score = score;
        }
    }
}
