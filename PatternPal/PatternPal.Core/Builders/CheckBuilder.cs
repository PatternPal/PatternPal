namespace PatternPal.Core.Builders;

internal delegate IEntity GetCurrentEntity(
    RecognizerContext ctx);

internal interface ICheckBuilder
{
    internal static readonly GetCurrentEntity GetCurrentEntity = ctx => ctx.CurrentEntity;

    ICheck Build();
}

internal class RootCheckBuilder : ICheckBuilder
{
    private ClassCheckBuilder ? _classCheckBuilder;

    public ICheck Build()
    {
        return new RootCheck();
    }

    internal RootCheckBuilder Class(
        Action< ClassCheckBuilder > buildAction)
    {
        _classCheckBuilder ??= new ClassCheckBuilder();
        buildAction(_classCheckBuilder);
        return this;
    }
}
