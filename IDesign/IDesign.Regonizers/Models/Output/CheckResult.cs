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
        public CheckCalculationType CalculationType { get; set; } = CheckCalculationType.Sum;
        public string Message { get; set; }
        public FeedbackType FeedbackType { get; set; }
        public INode Element { get; set; }
        public List<ICheckResult> ChildFeedback { get; set; } = new List<ICheckResult>();
        public IResourceMessage Feedback { get; set; }
        public bool HasIncorrectKnockOutCheck { get; set; }

        private float _score;

        public CheckResult(string message, FeedbackType feedbackType, INode element)
        {
            Message = message;
            FeedbackType = feedbackType;
            Element = element;
        }

        public CheckResult(IResourceMessage feedback, FeedbackType feedbackType, INode element)
        {
            Feedback = feedback;
            FeedbackType = feedbackType;
            Element = element;
        }

        public CheckResult(IResourceMessage feedback, FeedbackType feedbackType, INode element, float score)
        {
            Feedback = feedback;
            FeedbackType = feedbackType;
            Element = element;
            _score = score;
        }

        public CheckResult(string message, FeedbackType feedbackType, INode element, float score)
        {
            Message = message;
            FeedbackType = feedbackType;
            Element = element;
            _score = score;
        }

        public IEnumerable<ICheckResult> GetChildFeedback()
        {
            return ChildFeedback;
        }

        public float GetScore()
        {
            if (HasIncorrectKnockOutCheck) return 0;

            return ChildFeedback.Count == 0
                ? FeedbackType == FeedbackType.Correct ? _score : 0
                : CalculationType == CheckCalculationType.Average
                    ? ChildFeedback.Average(x => x.GetScore())
                    : ChildFeedback.Sum(x => x.GetScore());
        }

        public float GetTotalChecks()
        {
            return ChildFeedback.Count == 0
                ? _score
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
            return Feedback;
        }

        public void ChangeScore(float score)
        {
            _score = score;
        }
    }
}
