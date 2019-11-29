using Microsoft.CodeAnalysis;

namespace IDesign.Models
{
    public interface ICheckable
    {
        string GetSuggestionName();
        SyntaxNode GetSuggestionNode();
    }
}
