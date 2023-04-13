namespace PatternPal.Core.Builders;

internal class NotCheckBuilder : ICheckBuilder
{
    private readonly ICheckBuilder _checkBuilder;

    internal NotCheckBuilder(
        ICheckBuilder checkBuilder)
    {
        _checkBuilder = checkBuilder;
    }

    public ICheck Build() => new NotCheck(_checkBuilder.Build());
}
