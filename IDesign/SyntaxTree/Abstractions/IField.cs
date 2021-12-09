using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxTree.Abstractions {
    public interface IField : INode, IModified {
        TypeSyntax GetFieldType();
    }
}
