using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions.Entities;

namespace SyntaxTree.Abstractions.Members {
    public interface IField : INode, IModified, IChild<IEntity> {
        TypeSyntax GetFieldType();
    }
}
