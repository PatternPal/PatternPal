namespace SyntaxTree.Abstractions;

public class Relation
{
    private readonly RelationType _type;

    public OneOf< IEntity, IMethod > Source { get; }
    public OneOf< IEntity, IMethod > Target { get; }

    internal Relation(
        RelationType relationType,
        OneOf< IEntity, IMethod > source,
        OneOf< IEntity, IMethod > target)
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
            method => method.GetName());
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
    Method
}
