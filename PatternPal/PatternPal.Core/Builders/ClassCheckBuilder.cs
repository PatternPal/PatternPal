namespace PatternPal.Core.Builders;

internal interface IClassCheckBuilder : ICheckBuilder
{
    IClassCheckBuilder Method(
        Action< IMethodCheckBuilder > builderAction);
}

internal class ClassCheckBuilder : IClassCheckBuilder
{
    private IMethodCheckBuilder ? _methodCheckBuilder;

    public ICheck Build() => new ClassCheck();

    IClassCheckBuilder IClassCheckBuilder.Method(
        Action< IMethodCheckBuilder > builderAction)
    {
        _methodCheckBuilder ??= new MethodCheckBuilder();
        builderAction(_methodCheckBuilder);
        return this;
    }
}
