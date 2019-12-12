using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace IDesign.Recognizers
{
    public interface IMethod : ICheckable
    {
        string GetName();
        string GetReturnType();
        BlockSyntax GetBody();

        SyntaxTokenList GetModifiers();

        IEnumerable<string> GetParameterTypes();
    }
}