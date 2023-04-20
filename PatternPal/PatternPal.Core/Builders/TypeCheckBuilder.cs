namespace PatternPal.Core.Builders;

internal class TypeCheckBuilder : CheckBuilderBase
{
    private readonly OneOf< Func< List< INode > >, GetCurrentEntity > _getNode;

    internal TypeCheckBuilder(
        Priority priority,
        Func< List< INode > > getNode)
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
