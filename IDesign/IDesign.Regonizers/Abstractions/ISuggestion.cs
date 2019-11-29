using Microsoft.CodeAnalysis;

namespace IDesign.Recognizers.Abstractions
{
    public interface ISuggestion
    {
        string GetMessage();
        SyntaxNode GetSyntaxNode();
    }
}