using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxTree.Abstractions.Members
{
    public interface IProperty : IMember
    {
        bool HasGetter();
        bool HasSetter();

        IMethod GetGetter();
        IMethod GetSetter();

        bool IsField();

        IField GetField();

        TypeSyntax GetPropertyType();
    }
}
