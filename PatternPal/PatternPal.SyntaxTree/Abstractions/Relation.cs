using PatternPal.SyntaxTree.Abstractions.Members;

namespace PatternPal.SyntaxTree.Abstractions;

/// <summary>
/// Represents a relation of type <see cref="RelationType"/> from <see cref="Source"/> to <see cref="Target"/>.
/// </summary>
public class Relation
{
    // The type of relation.
    private readonly RelationType _type;

    // The source of the relation.
    public OneOf< IEntity, IMember > Source { get; }

    // The target of the relation.
    public OneOf< IEntity, IMember > Target { get; }

    /// <summary>
    /// Returns an instance of <see cref="Relation"/>.
    /// </summary>
    /// <param name="relationType">The type of relation.</param>
    /// <param name="source">The source of the relation.</param>
    /// <param name="target">The target of the relation.</param>
    internal Relation(
        RelationType relationType,
        OneOf< IEntity, IMember > source,
        OneOf< IEntity, IMember > target)
    {
        _type = relationType;
        Source = source;
        Target = target;
    }

    /// <summary>
    /// Gets the RelationType of this Relation.
    /// </summary>
    /// <returns>The RelationType of this Relation.</returns>
    public RelationType GetRelationType()
    {
        return _type;
    }

    /// <summary>
    /// Gets the name of the destination node of this relation.
    /// </summary>
    /// <returns>The name of the destination node of this relation.</returns>
    public string GetDestinationName()
    {
        return Target.Match(
            entity => entity.GetName(),
            member => member.GetName());
    }
}

/// <summary>
/// The types of relations which are possible between entity and method nodes in the SyntaxGraph.
/// </summary>
public enum RelationType
{
    Implements,
    ImplementedBy,
    Extends,
    ExtendedBy,
    Overrides,
    OverriddenBy,
    Uses,
    UsedBy,
    Creates,
    CreatedBy
}

/// <summary>
/// This enum denotes which type the destination node has to be when querying on sets of relations.
/// </summary>
public enum RelationTargetKind
{
    All,
    Entity,
    Member
}
