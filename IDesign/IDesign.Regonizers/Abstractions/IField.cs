using IDesign.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers
{
    public interface IField : ICheckable
    {
        string GetName();
        TypeSyntax GetFieldType();
        SyntaxTokenList GetModifiers();
    }
}