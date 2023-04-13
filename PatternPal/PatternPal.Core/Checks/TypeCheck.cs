namespace PatternPal.Core.Checks;

internal class TypeCheck : ICheck
{
    //used to get the type
    private readonly Func<INode> _getNode;

    public TypeCheck(
        Func<INode> getNode)
    {
        _getNode = getNode;
    }

    public ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        throw new NotImplementedException("Type Check was not implemented");
    }
}
