using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxTree.Abstractions.Members {
    public interface IField : IMember {
        TypeSyntax GetFieldType();
    }
}
