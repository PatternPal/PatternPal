namespace PatternPal.SyntaxTree.Abstractions.Root
{
    /// <summary>
    /// A root represents a compiled file in the <see cref="SyntaxGraph" />.
    /// </summary>
    public interface IRoot : IEntitiesContainer, INamespaceContainer, IUsingContainer
    {
        /// <summary>
        /// Gets the filepath of the source file where this <see cref="INode"/> is found in.
        /// </summary>
        string GetSource();

        /// <summary>
        /// Gets all <see cref="Relation"/>s from an <see cref="INode"/>, filtered on the type of the destination <see cref="INode"/> of the relation.
        /// </summary>
        IEnumerable<Relation> GetRelations(INode node, RelationTargetKind type);
    }

    /// <summary>
    /// Represents an <see cref="INode"/> which contains <see cref="INamespace"/>s.
    /// </summary>
    public interface INamespaceContainer : INode, IParent
    {
        /// <summary>
        /// Gets all <see cref="INamespace"/>s in the current <see cref="INode"/>.
        /// </summary>
        IEnumerable<INamespace> GetNamespaces();
    }

    /// <summary>
    /// Represents an <see cref="INode"/> which has usings.
    /// </summary>
    public interface IUsingContainer : INode
    {
        /// <summary>
        /// Gets all usings in the current <see cref="INode"/>.
        /// </summary>
        IEnumerable<UsingDirectiveSyntax> GetUsing();
    }

    /// <summary>
    /// Represents an <see cref="INode"/> which has <see cref="IEntity"/>s.
    /// </summary>
    public interface IEntitiesContainer : INode, IParent
    {
        /// <summary>
        /// Get all <see cref="IEntity"/>s in the current <see cref="INode"/>, not in sub namespaces.
        /// </summary>
        IEnumerable<IEntity> GetEntities();

        /// <summary>
        /// Get all <see cref="IEntity"/>s mapped by their fullname.
        /// This looks in sub namespaces.
        /// </summary>
        Dictionary<string, IEntity> GetAllEntities();
    }

    /// <inheritdoc />
    public interface INamedEntitiesContainer : IEntitiesContainer
    {
        /// <summary>
        /// Gets the name of the <see cref="IEntity"/> preceded by the namespace that contains it.
        /// </summary>
        string GetNamespace();
    }
}
