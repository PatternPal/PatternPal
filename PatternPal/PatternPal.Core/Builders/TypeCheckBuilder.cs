namespace PatternPal.Core.Builders;
internal class TypeCheckBuilder : CheckBuilderBase
{
    //used to get the type
    private readonly Func<INode> _getNode;

    internal TypeCheckBuilder(Priority priority,
        Func<INode> getNode) : base(priority)
    {
        _getNode = getNode;
    }

    public override ICheck Build() => new TypeCheck(Priority, _getNode);
}
