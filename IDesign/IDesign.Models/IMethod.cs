using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Models
{
    public interface IMethod : ICheckable
    {
        string GetName();
        string GetReturnType();
        BlockSyntax GetBody();

        SyntaxTokenList GetModifiers();
    }
}