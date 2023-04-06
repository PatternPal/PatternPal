namespace PatternPal.Core.Builders;

internal delegate IEntity GetCurrentEntity(
    RecognizerContext ctx);

internal interface ICheckBuilder
{
    internal static readonly GetCurrentEntity GetCurrentEntity = ctx => ctx.CurrentEntity;

    ICheck Build();
}

internal interface IRootCheckBuilder : ICheckBuilder
{
    IRootCheckBuilder Class(
        Action< IClassCheckBuilder > builderAction);
}

internal class RootCheckBuilder : IRootCheckBuilder
{
    private ClassCheckBuilder ? _classCheckBuilder;

    public ICheck Build()
    {
        return new RootCheck();
    }

    IRootCheckBuilder IRootCheckBuilder.Class(
        Action< IClassCheckBuilder > buildAction)
    {
        _classCheckBuilder ??= new ClassCheckBuilder();
        buildAction(_classCheckBuilder);
        return this;
    }
}
