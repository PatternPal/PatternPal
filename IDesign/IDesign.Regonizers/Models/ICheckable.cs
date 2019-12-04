using Microsoft.CodeAnalysis;

namespace IDesign.Recognizers
{
    public interface ICheckable
    {
        string GetSuggestionName();
        SyntaxNode GetSuggestionNode();
    }
}
