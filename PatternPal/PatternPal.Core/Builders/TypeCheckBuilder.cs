namespace PatternPal.Core.Builders;

internal class TypeCheckBuilder : CheckBuilderBase
{
    private readonly OneOf< Func< INode >, GetCurrentEntity > _getNode;

    internal TypeCheckBuilder(
        Priority priority,
        Func< INode > getNode)
        : base(priority)
    {
        _getNode = getNode;
    }

    internal TypeCheckBuilder(
        Priority priority,
        GetCurrentEntity getCurrentEntity)
        : base(priority)
    {
        _getNode = getCurrentEntity;
    }

    public override ICheck Build() => new TypeCheck(
        Priority,
        _getNode);
}
