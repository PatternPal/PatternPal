using Microsoft.CodeAnalysis;

namespace IDesign.Recognizers.Models
{
    public interface ICheckable
    {
        string GetSuggestionName();
        SyntaxNode GetSuggestionNode();
    }
}