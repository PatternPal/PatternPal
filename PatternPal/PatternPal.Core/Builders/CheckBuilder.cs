namespace PatternPal.Core.Builders;

internal delegate IEntity GetCurrentEntity(
    RecognizerContext ctx);

internal interface ICheckBuilder
{
    internal static readonly GetCurrentEntity GetCurrentEntity = ctx => ctx.CurrentEntity;

    ICheck Build();

    internal static TParentCheckBuilder BuildCheck< TParentCheckBuilder, TChildCheckBuilder >(
        TParentCheckBuilder classCheckBuilder,
        ref TChildCheckBuilder ? methodCheckBuilder,
        Action< TChildCheckBuilder > builderAction)
        where TChildCheckBuilder : ICheckBuilder, new()
    {
        methodCheckBuilder ??= new TChildCheckBuilder();
        builderAction(methodCheckBuilder);
        return classCheckBuilder;
    }
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
