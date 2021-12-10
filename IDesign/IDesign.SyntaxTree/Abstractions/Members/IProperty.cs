using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions.Entities;

namespace SyntaxTree.Abstractions.Members {
    public interface IProperty : INode, IModified, IChild<IEntity> {
        bool HasGetter();
        bool HasSetter();

        IMethod GetGetter();
        IMethod GetSetter();
        
        bool IsField();

        IField GetField();
        
        TypeSyntax GetPropertyType();
    }
}
