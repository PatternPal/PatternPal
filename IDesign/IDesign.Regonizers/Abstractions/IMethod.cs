using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers
{
    public interface IMethod
    {
        string GetName();
        string GetReturnType();
        BlockSyntax GetBody();

        SyntaxTokenList GetModifiers();
    }
}