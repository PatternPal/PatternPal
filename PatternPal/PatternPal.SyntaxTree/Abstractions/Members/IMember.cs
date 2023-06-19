namespace PatternPal.SyntaxTree.Abstractions.Members
{
    /// <summary>
    /// A member of an <see cref="IEntity"/>, like constructors, fields etc.
    /// </summary>
    public interface IMember : IModified, IChild<IEntity>
    {
        /// <summary>
        /// Returns, if available, the return type of the member.
        /// Else it returns the type of the member.
        /// </summary>
        /// <returns>The <see cref="SyntaxNode"/> of the return type of the member.</returns>
        SyntaxNode GetReturnType();
    }
}
