namespace PatternPal.Core.Checks;

/// <summary>
/// Checks for a uses relation of an entity. Depending on the <see cref="Func{INode}"/> provided.
/// It should go from the current <see cref="INode"/> being checked to the <see cref="Func{INode}"/> provided.
/// </summary>
internal class UsesCheck : CheckBase
{
    //the node resulting from a check to which there should be a uses relation
    //made a Func<> so that it works with lazy evaluation
    private readonly Func< INode > _getNode;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsesCheck"/> class.
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="getNode">A functor to get the node to check for a uses relation</param>
    internal UsesCheck(Priority priority,
        Func< INode > getNode) : base(priority)
    {
        _getNode = getNode;
    }

    /// <summary>
    /// This check is marked as correct when the given <paramref name="node"/>
    /// uses the node provided.
    /// </summary>
    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        bool hasUsesRelation = ctx.Graph.GetRelations(node, RelationTargetKind.All). //get all relations the checked node has
            Any(relation => relation.GetRelationType() == RelationType.Uses && //which are of type uses
                            relation.Target.Match(entity => entity == _getNode(), method => method == _getNode())); //and go to the node which should be used

        return hasUsesRelation 
            ? new LeafCheckResult
              {
                Correctness = true, 
                Priority = Priority, 
                FeedbackMessage = $"Node {node} correctly uses node {_getNode()}"

              } 
            : new LeafCheckResult
              { 
                Correctness = false, 
                Priority = Priority,
                FeedbackMessage = $"No uses relation found."
              };
    }
}
