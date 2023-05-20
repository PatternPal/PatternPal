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

        /// <summary>
        /// Returns whether the property is an auto property.
        /// This means that it has an empty getter, and that if
        /// it has a setter, it is empty as well.
        /// </summary>
        bool IsField();

        /// <summary>
        /// Gets the property wrapped in an <see cref="IField"/>.
        /// </summary>
        IField GetField();

        /// <summary>
        /// Gets the Roslyn representation of the type of the property.
        /// </summary>
        TypeSyntax GetPropertyType();
    }
}
