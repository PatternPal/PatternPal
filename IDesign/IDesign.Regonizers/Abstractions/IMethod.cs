using IDesign.Recognizers.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers.Abstractions
{
    public interface IMethod : ICheckable
    {
        string GetName();
        string GetReturnType();
        BlockSyntax GetBody();
        SyntaxTokenList GetModifiers();
        ParameterListSyntax GetParameter();
    }
}