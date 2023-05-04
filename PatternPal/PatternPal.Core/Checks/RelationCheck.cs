namespace PatternPal.Core.Checks;

/// <summary>
/// Checks for a relation of an entity. Depending on the <see cref="RelationType"/> and <see cref="Func{INode}"/> provided.
/// It should go from the current <see cref="INode"/> being checked to the <see cref="Func{INode}"/> provided.
/// </summary>
internal class RelationCheck : CheckBase
{
    // The node resulting from a check to which there should be a relation,
    // made a Func<> so that it works with lazy evaluation.
    private readonly Func< List<INode> > _getNodes;

    // The type of relation which should be present.
    private readonly RelationType _relationType;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelationCheck"/> class.
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="relationType">The type of relation.</param>
    /// <param name="getNodes">A functor to get the node to check for a <see cref="_relationType"/> relation.</param>
    internal RelationCheck(Priority priority,
        RelationType relationType,
        Func< List<INode> > getNodes) : base(priority)
    {
        _getNodes = getNodes;
        _relationType = relationType;
    }

    /// <summary>
    /// This check is marked as correct when the given <paramref name="node"/>
    /// has a <see cref="_relationType"/> relation with the node provided.
    /// </summary>
    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        List < ICheckResult > results = new();
        foreach (INode getNode in _getNodes())
        {
            bool hasCorrectRelation = ctx.Graph.GetRelations(node, RelationTargetKind.All). //get all relations the checked node has
                Any(relation => relation.GetRelationType() == _relationType && //which are of type uses
                                relation.Target.Match(entity => entity == getNode, method => method == getNode)); //and go to the node which should be used

            results.Add(new LeafCheckResult
                {
                    Correct = hasCorrectRelation,
                    Priority = Priority,
                    FeedbackMessage = hasCorrectRelation ? $"Node {node} has a {_relationType} relation with node {getNode}" : $"No {_relationType} relation found."
                });
        }
        

        return new NodeCheckResult
              {
                Priority = Priority,
                ChildrenCheckResults = results,
                FeedbackMessage = $"Found {_relationType} relations for {node}."
              };
    }
}
