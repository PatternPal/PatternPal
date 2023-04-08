namespace PatternPal.Core.Checks;

internal class UsesCheck : ICheck
{
    private readonly Func< INode > _getNode;

    internal UsesCheck(
        Func< INode > getNode)
    {
        _getNode = getNode;
    }

    public bool Check(
        RecognizerContext ctx,
        INode node)
    {
        if (node is not IMethod method)
        {
            return false;
        }

        if (_getNode() is not IMethod usedMethod)
        {
            return false;
        }

        if (method == usedMethod)
        {
            return false;
        }

        Console.WriteLine("got uses check");
        foreach (IRelation relation in ctx.Graph.GetRelations(method.GetParent()))
        {
            if (relation.GetRelationType() == RelationType.Uses
                && relation.GetDestination() == usedMethod.GetParent())
            {
                Console.WriteLine($"Used method: {usedMethod}");
                return true;
            }
        }

        return false;
    }
}
