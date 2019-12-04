using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace IDesign.Models
{
    public interface IMethod : ICheckable
    {
        string GetName();
        string GetReturnType();
        BlockSyntax GetBody();
        List<string> GetParameters();
        SyntaxTokenList GetModifiers();
    }
}