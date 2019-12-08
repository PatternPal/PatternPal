using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis;

namespace IDesign.Recognizers.Models.Output
{
    public class Feedback : IFeedback
    {
        public Feedback(string message, FeedbackType feedbackType, SyntaxNode node)
        {
            Message = message;
            FeedbackType = feedbackType;
            Node = node;
        }

        public string Message { get; set; }
        public FeedbackType FeedbackType { get; set; }
        public SyntaxNode Node { get; set; }
        public List<Feedback> ChildFeedback { get; set; }



        public string GetMessage()
        {
            return Message;
        }

        public IEnumerable<IFeedback> GetChildFeedback()
        {
            return ChildFeedback;
        }

        public int GetScore()
        {
            if (!ChildFeedback.Any())
                return FeedbackType == FeedbackType.Correct ? 1 : 0;
            return ChildFeedback.Sum(x => x.GetScore());
        }
        
        public FeedbackType GetFeedbackType()
        {
            return FeedbackType;
        }

        public SyntaxNode GetSyntaxNode()
        {
            return Node;
        }
    }
}