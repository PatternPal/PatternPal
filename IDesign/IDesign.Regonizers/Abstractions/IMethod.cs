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
        IEnumerable<string> GetArguments();
        BlockSyntax GetBody();
        IEnumerable<string> GetParameterTypes();
        IEnumerable<ParameterSyntax> GetParameters();
        SyntaxTokenList GetModifiers();
        ParameterListSyntax GetParameter();
    }
}