namespace PatternPal.Core.Checks;

internal class TypeCheck : CheckBase
{
    //used to get the type
    private readonly Func<INode> _getNode;

    public TypeCheck(Priority priority,
        Func<INode> getNode) : base(priority)
    {
        _getNode = getNode;
    }

    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        throw new NotImplementedException("Type Check was not implemented");
    }
}
