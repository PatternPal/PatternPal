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

        if (ctx.Graph.GetRelations(method, Relationable.Method).Any(x => x.Node2Method == usedMethod))
        {
            Console.WriteLine($"Used method: '{usedMethod}' Used by: '{method}'");
            return true;
        }

        return false;
    }
}
