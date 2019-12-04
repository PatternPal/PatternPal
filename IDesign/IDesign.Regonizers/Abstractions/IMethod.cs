using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers
{
    public interface IMethod : ICheckable
    {
        string GetName();
        string GetReturnType();
        BlockSyntax GetBody();

        SyntaxTokenList GetModifiers();
    }
}