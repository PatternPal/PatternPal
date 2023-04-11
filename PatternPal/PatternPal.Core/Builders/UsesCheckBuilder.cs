namespace PatternPal.Core.Builders;

// TODO CV: Relations can probably use the same check builder
internal class UsesCheckBuilder : ICheckBuilder
{
    private readonly Func< INode > _getNode;

    internal UsesCheckBuilder(
        Func< INode > getNode)
    {
        _getNode = getNode;
    }

    public ICheck Build() => new UsesCheck(_getNode);
}
