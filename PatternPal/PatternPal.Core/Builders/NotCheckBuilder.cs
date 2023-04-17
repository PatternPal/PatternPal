namespace PatternPal.Core.Builders;

internal class NotCheckBuilder : CheckBuilderBase
{
    private readonly ICheckBuilder _checkBuilder;

    internal NotCheckBuilder(Priority priority,
        ICheckBuilder checkBuilder) : base(priority)
    {
        _checkBuilder = checkBuilder;
    }

    public override ICheck Build() => new NotCheck(Priority, _checkBuilder.Build());
}
