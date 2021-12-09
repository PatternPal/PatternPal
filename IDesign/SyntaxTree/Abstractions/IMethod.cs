using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxTree.Abstractions {
    public interface IMethod : INode, IModified, IParameterized, IBodied {
        TypeSyntax GetReturnType();
    }
}
