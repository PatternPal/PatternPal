namespace PatternPal.Core.Checks;

/// <summary>
/// Checks for a uses relation of an entity. Depending on the <see cref="Func{INode}"/> provided.
/// It should go from the current <see cref="INode"/> being checked to the <see cref="Func{INode}"/> provided.
/// </summary>
internal class UsesCheck : CheckBase
{
    //the node resulting from a check to which there should be a uses relation
    //made a Func<> so that it works with lazy evaluation
    private readonly Func< List<INode> > _getNodes;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsesCheck"/> class.
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="getNodes">A functor to get the node to check for a uses relation</param>
    internal UsesCheck(Priority priority,
        Func< List<INode> > getNodes) : base(priority)
    {
        _getNodes = getNodes;
    }

    /// <summary>
    /// This check is marked as correct when the given <paramref name="node"/>
    /// uses the node provided.
    /// </summary>
    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        List < ICheckResult > usesResults = new();
        foreach (INode getNode in _getNodes())
        {
            bool hasUsesRelation = ctx.Graph.GetRelations(node, RelationTargetKind.All). //get all relations the checked node has
                Any(relation => relation.GetRelationType() == RelationType.Uses && //which are of type uses
                                relation.Target.Match(entity => entity == getNode, method => method == getNode)); //and go to the node which should be used

                usesResults.Add(new LeafCheckResult
                {
                    Correct = hasUsesRelation,
                    Priority = Priority,
                    FeedbackMessage = hasUsesRelation ? $"Node {node} correctly uses node {getNode}" : $"No uses relation found."
                });
        }
        

        return new NodeCheckResult
              {
                Priority = Priority,
                ChildrenCheckResults = usesResults,
                FeedbackMessage = $"Found uses relations for {node}."
              };
    }
}
