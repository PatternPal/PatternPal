using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxTree.Abstractions {
    public interface INode {
        string GetName();

        SyntaxNode GetSyntaxNode();
    }
    
    public interface IModified {
        IEnumerable<IModifier> GetModifiers();
    }
    
    public interface IParameterized {
        IEnumerable<TypeSyntax> GetParameters();
    }
    
    public interface IBodied {
        CSharpSyntaxNode GetBody();
    }
}
