using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Models
{
    public interface IMethod
    {
        string GetName();
        TypeSyntax GetReturnType();
        BlockSyntax GetBody();

        SyntaxTokenList GetModifiers();

    }
}
