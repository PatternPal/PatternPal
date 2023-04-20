namespace PatternPal.Core.Builders;

// TODO CV: Relations can probably use the same check builder
internal class UsesCheckBuilder : CheckBuilderBase
{
    private readonly Func< List<INode> > _getNodes;

    internal UsesCheckBuilder(Priority priority,
        Func< List<INode> > getNodes) : base(priority)
    {
        _getNodes = getNodes;
    }

    public override ICheck Build() => new UsesCheck(Priority, _getNodes);
}
