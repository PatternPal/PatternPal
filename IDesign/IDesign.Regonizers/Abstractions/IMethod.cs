using IDesign.Recognizers.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace IDesign.Recognizers.Abstractions
{
    public interface IMethod : ICheckable
    {
        string GetName();
        string GetReturnType();
        BlockSyntax GetBody();
        SyntaxTokenList GetModifiers();
        IEnumerable<string> GetParameterTypes();
        ParameterListSyntax GetParameter();
    }
}