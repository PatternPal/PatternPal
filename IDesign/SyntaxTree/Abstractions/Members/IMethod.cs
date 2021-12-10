using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions.Entities;

namespace SyntaxTree.Abstractions.Members {
    public interface IMethod : INode, IModified, IParameterized, IBodied, IChild<IEntity> {
        TypeSyntax GetReturnType();
    }
}
