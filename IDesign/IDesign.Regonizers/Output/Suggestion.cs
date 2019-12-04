using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis;

namespace IDesign.Recognizers.Output
{
    public class Suggestion : ISuggestion
    {
        public Suggestion(string message, SyntaxNode node)
        {
            Message = message;
            Node = node;
        }

        public string Message { get; set; }
        public SyntaxNode Node { get; set; }

        public string GetMessage()
        {
            return Message;
        }

        public SyntaxNode GetSyntaxNode()
        {
            return Node;
        }
    }
}