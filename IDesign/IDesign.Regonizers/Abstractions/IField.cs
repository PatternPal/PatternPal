using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers
{
    public interface IField
    {
        string GetName();
        TypeSyntax GetFieldType();
        SyntaxTokenList GetModifiers();
    }
}