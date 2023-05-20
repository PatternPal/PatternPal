namespace PatternPal.SyntaxTree.Abstractions.Members
{
    /// <summary>
    /// An <see cref="INode"/> which represents a field.
    /// </summary>
    public interface IField : IMember
    {
        /// <summary>
        /// Gets the Roslyn representation of the type of the field.
        /// </summary>
        TypeSyntax GetFieldType();
    }
}
