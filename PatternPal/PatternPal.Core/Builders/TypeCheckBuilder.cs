namespace PatternPal.Core.Builders;

internal class TypeCheckBuilder : CheckBuilderBase
{
    private readonly GetCurrentEntity ? _getCurrentEntity;
    private readonly Func< INode > ? _getNode;

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
        _getCurrentEntity = getCurrentEntity;
    }

    public override ICheck Build() => new TypeCheck(
        Priority,
        _getNode,
        _getCurrentEntity);
}
