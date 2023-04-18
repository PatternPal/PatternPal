namespace PatternPal.Core.Builders;

internal class PropertyCheckBuilder : CheckCollectionBuilder
{
    internal PropertyCheckBuilder(Priority priority,
        IEnumerable< ICheckBuilder > checkBuilders)
        : base(priority,
            CheckCollectionKind.All,
            checkBuilders)
    {
    }

    public override ICheck Build() => new PropertyCheck(Priority, CheckBuilders.Select(checkBuilder => checkBuilder.Build()));
}
