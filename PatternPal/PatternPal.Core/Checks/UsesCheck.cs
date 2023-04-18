namespace PatternPal.Core.Checks;

internal class UsesCheck : CheckBase
{
    //the node resulting from a check to which there should be a uses relation
    //made a Func<> so that it works with lazy evaluation
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
