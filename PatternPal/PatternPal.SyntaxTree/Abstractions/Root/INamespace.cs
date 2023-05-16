namespace PatternPal.SyntaxTree.Abstractions.Root
{
    public interface INamespace
        : INamespaceContainer, IUsingContainer, INamedEntitiesContainer, IChild<INamespaceContainer>
    {
    }
}
