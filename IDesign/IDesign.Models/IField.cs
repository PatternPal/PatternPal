using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Models
{
    public interface IField : ICheckable
    {
        string GetName();
        TypeSyntax GetFieldType();
        SyntaxTokenList GetModifiers();
    }
}