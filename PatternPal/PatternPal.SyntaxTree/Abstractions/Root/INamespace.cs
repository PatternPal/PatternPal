namespace PatternPal.SyntaxTree.Abstractions.Root
{
    /// <summary>
    /// An <see cref="INode"/> which represents a namespace.
    /// </summary>
    public interface INamespace
        : INamespaceContainer, IUsingContainer, INamedEntitiesContainer, IChild<INamespaceContainer>
    {
    }
}
