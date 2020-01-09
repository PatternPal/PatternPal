using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis;

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

        public CheckResult(string message, FeedbackType feedbackType, SyntaxNode node)
        {
            Message = message;
            FeedbackType = feedbackType;
            Node = node;
        }

        public CheckResult(string message, FeedbackType feedbackType, SyntaxNode node, float score)
        {
            Message = message;
            FeedbackType = feedbackType;
            Node = node;
            Score = score;
        }

        public string Message { get; set; }
        public FeedbackType FeedbackType { get; set; }
        public SyntaxNode Node { get; set; }
        public List<ICheckResult> ChildFeedback { get; set; } = new List<ICheckResult>();
        public float Score { get; set; }

        public string GetMessage()
        {
            return Message;
        }

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

            if (ChildFeedback.Any(x => x.GetFeedbackType() == FeedbackType.Correct))
            {
                feedback = FeedbackType.SemiCorrect;
            }

            if (ChildFeedback.All(x => x.GetFeedbackType() == FeedbackType.Correct))
            {
                feedback = FeedbackType.Correct;
            }

            return feedback;
        }

        public SyntaxNode GetSyntaxNode()
        {
            return Node;
        }

        public void ChangeScore(float score)
        {
            Score = score;
        }
    }
}