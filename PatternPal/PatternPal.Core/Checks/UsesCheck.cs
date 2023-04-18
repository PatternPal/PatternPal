namespace PatternPal.Core.Checks;

internal class UsesCheck : CheckBase
{
    private readonly Func< INode > _getNode;

    internal UsesCheck(Priority priority,
        Func< INode > getNode) : base(priority)
    {
        _getNode = getNode;
    }

    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        bool hasUsesRelation = ctx.Graph.GetRelations(node, RelationTargetKind.All).
            Any(relation => relation.GetRelationType() == RelationType.Uses && 
                            relation.Target.Match(entity => entity == _getNode(), method => method == _getNode()));

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
