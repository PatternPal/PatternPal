using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxTree.Abstractions {
    public interface IProperty : INode, IModified {
        bool HasGetter();
        bool HasSetter();

        IMethod GetGetter();
        IMethod GetSetter();
        
        bool IsField();

        IField GetField();
        
        TypeSyntax GetPropertyType();
    }
}
