namespace PatternPal.Core.Checks;

/// <summary>
/// Checks for a relation of an entity. Depending on the <see cref="RelationType"/> and <see cref="Func{INode}"/> provided.
/// It should go from the current <see cref="INode"/> being checked to the <see cref="Func{INode}"/> provided.
/// </summary>
internal class RelationCheck : CheckBase
{
    // The check which checks for the node to which there should be a relation.
    private readonly ICheck _relatedNodeCheck;

    private Score _perfectScore;

    // The type of relation which should be present.
    private readonly RelationType _relationType;

    /// <summary>
    /// A <see cref="RelationCheck"/> is dependent on the <see cref="INode"/> to which it has an <see cref="Relation"/>.
    /// </summary>
    public override int DependencyCount => 1 + _relatedNodeCheck.DependencyCount;

    /// <inheritdoc />
    public override Score PerfectScore => _perfectScore.Equals(default)
        ? _perfectScore = PerfectScoreFromRelatedCheck(_relatedNodeCheck)
                          + Score.CreateScore(
                              Priority,
                              true)
        : _perfectScore;

    /// <summary>
    /// Gets the perfect <see cref="Score"/> of the <paramref name="relatedCheck"/>.
    /// </summary>
    /// <param name="relatedCheck">The related <see cref="ICheck"/>.</param>
    /// <returns>The <see cref="ICheck.PerfectScore"/> of the <paramref name="relatedCheck"/>.</returns>
    internal static Score PerfectScoreFromRelatedCheck(
        ICheck relatedCheck)
    {
        while (true)
        {
            if (relatedCheck is ClassCheck or InterfaceCheck)
            {
                return relatedCheck.PerfectScore;
            }

            if (relatedCheck.ParentCheck == null)
            {
                return default;
            }
            relatedCheck = relatedCheck.ParentCheck;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RelationCheck"/> class.
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="requirement">The optional requirement which this <see cref="ICheck"/> checks.</param>
    /// <param name="relationType">The type of relation.</param>
    /// <param name="relatedNodeCheck">The <see cref="ICheck"/> which checks for the node
    /// to which there should be a <see cref="_relationType"/> relation.</param>
    internal RelationCheck(
        Priority priority,
        string ? requirement,
        RelationType relationType,
        ICheck relatedNodeCheck)
        : base(
            priority,
            requirement)
    {
        _relatedNodeCheck = relatedNodeCheck;
        _relationType = relationType;
    }

    /// <summary>
    /// This check is marked as correct when the given <paramref name="node"/>
    /// has a <see cref="_relationType"/> relation with the node provided.
    /// </summary>
    public override ICheckResult Check(
        IRecognizerContext ctx,
        INode node)
    {
        List< ICheckResult > results = new();
        foreach (INode getNode in _relatedNodeCheck.Result())
        {
            bool hasCorrectRelation = ctx.Graph.GetRelations(
                node,
                RelationTargetKind.All).Any(
                relation => relation.GetRelationType() == _relationType
                            && relation.Target.Match(
                                entity => entity == getNode,
                                method => method == getNode));

            results.Add(
                new LeafCheckResult
                {
                    Correct = hasCorrectRelation,
                    Priority = Priority,
                    FeedbackMessage = hasCorrectRelation
                        ? $"Node {node} has a {_relationType} relation with node {getNode}"
                        : $"No {_relationType} relation found.",
                    DependencyCount = DependencyCount,
                    MatchedNode = getNode,
                    Check = this,
                    RelatedCheck = _relatedNodeCheck
                });
        }

        return new NodeCheckResult
               {
                   Priority = Priority,
                   ChildrenCheckResults = results,
                   NodeCheckCollectionWrapper = true,
                   FeedbackMessage = $"Found {_relationType} relations for {node}.",
                   DependencyCount = DependencyCount,
                   MatchedNode = node,
                   Check = this,
                   CollectionKind = CheckCollectionKind.Any,
               };
    }
}
