using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternPal.SyntaxTree.Abstractions.Members
{
    /// <summary>
    /// An <see cref="INode"/> which represents a property.
    /// </summary>
    public interface IProperty : IMember
    {
        /// <summary>
        /// Whether the property has a getter.
        /// </summary>
        bool HasGetter();

        /// <summary>
        /// Whether the property has a setter.
        /// </summary>
        bool HasSetter();

        /// <summary>
        /// Returns the getter part of the property as an <see cref="IMethod"/>.
        /// </summary>
        IMethod GetGetter();

        /// <summary>
        /// Returns the setter part of the property as an <see cref="IMethod"/>.
        /// </summary>
        IMethod GetSetter();

        bool IsField();

        /// <summary>
        /// Gets the property rapped in an <see cref="IField"/>.
        /// </summary>
        IField GetField();

        /// <summary>
        /// Gets the Roslyn representation of the type of the property.
        /// </summary>
        TypeSyntax GetPropertyType();
    }
}
