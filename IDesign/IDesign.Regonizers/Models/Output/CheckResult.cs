using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis;

namespace IDesign.Recognizers.Models.Output
{
    public class CheckResult : ICheckResult
    {
        public CheckResult(string message, FeedbackType feedbackType, ICheckable element)
        {
            Message = message;
            FeedbackType = feedbackType;
            Element = element;
        }


        public CheckResult(IResourceMessage feedback, FeedbackType feedbackType, ICheckable element)
        {
            _feedback = feedback;
            FeedbackType = feedbackType;
            Element = element;
        }

        public CheckResult(IResourceMessage feedback, FeedbackType feedbackType, ICheckable element, int score)
        {
            _feedback = feedback;
            FeedbackType = feedbackType;
            Element = element;
            Score = score;
        }
        public CheckResult(string message, FeedbackType feedbackType, SyntaxNode node, float score)

        {
            Message = message;
            FeedbackType = feedbackType;
            Element = element;
            Score = score;

        }

        public string Message { get; set; }
        public FeedbackType FeedbackType { get; set; }
        public ICheckable Element { get; set; }
        public List<ICheckResult> ChildFeedback { get; set; } = new List<ICheckResult>();


        public IResourceMessage _feedback { get; set; }

        public float Score { get; set; }




        public IEnumerable<ICheckResult> GetChildFeedback()
        {
            return ChildFeedback;
        }

        public float GetScore()
        {
            if (!ChildFeedback.Any())
                return FeedbackType == FeedbackType.Correct ? Score : 0;

            return ChildFeedback.Sum(x => x.GetScore());
        }

        public float GetTotalChecks()
        {
            if (ChildFeedback.Count == 0)
            {
                return Score;
            }
            return ChildFeedback.Sum(x => x.GetTotalChecks());
        }

        public FeedbackType GetFeedbackType()
        {
            if (ChildFeedback.Count == 0)
            {
                return FeedbackType;
            }

            var feedback = FeedbackType.Incorrect;

            if (ChildFeedback.Any(x => x.GetFeedbackType() == FeedbackType.Correct))
                feedback = FeedbackType.SemiCorrect;
            if (ChildFeedback.All(x => x.GetFeedbackType() == FeedbackType.Correct))
                feedback = FeedbackType.Correct;

            return feedback;
        }

        public ICheckable GetElement()
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
