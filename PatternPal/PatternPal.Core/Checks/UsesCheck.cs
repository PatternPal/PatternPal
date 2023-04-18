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
        if (node is not IMethod method)
        {
            throw new NotImplementedException("Uses Check was incorrect");
        }

        if (_getNode() is not IMethod usedMethod)
        {
            throw new NotImplementedException("Uses Check was incorrect");
        }

        if (method == usedMethod)
        {
            throw new NotImplementedException("Uses Check was incorrect");
        }

        foreach (IRelation relation in ctx.Graph.GetRelations(method.GetParent()))
        {
            if (relation.GetRelationType() == RelationType.Uses
                && relation.GetDestination() == usedMethod.GetParent()
                && method.GetParent() != usedMethod.GetParent())
            {
                Console.WriteLine($"Used method: '{usedMethod}' Used by: '{method}'");
                throw new NotImplementedException("Uses Check was correct");
            }
        }

        throw new NotImplementedException("Uses Check was incorrect");
    }
}
