using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternPal.SyntaxTree.Abstractions.Members
{
    /// <summary>
    /// An <see cref="INode"/> which represents a property.
    /// </summary>
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
