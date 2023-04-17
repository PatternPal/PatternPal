namespace PatternPal.Core.Builders;

// TODO CV: Relations can probably use the same check builder
internal class UsesCheckBuilder : CheckBuilderBase
{
    private readonly Func< INode > _getNode;

    internal UsesCheckBuilder(Priority priority,
        Func< INode > getNode) : base(priority)
    {
        _getNode = getNode;
    }

    public override ICheck Build() => new UsesCheck(Priority, _getNode);
}
