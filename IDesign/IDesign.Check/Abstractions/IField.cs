using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Checks
{
    public interface IField
    {
        string GetName();
        TypeSyntax GetFieldType();
        SyntaxTokenList GetModifiers();
    }
}