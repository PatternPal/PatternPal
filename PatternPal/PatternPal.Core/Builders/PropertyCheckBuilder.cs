namespace PatternPal.Core.Builders;

internal class PropertyCheckBuilder : CheckCollectionBuilder
{
    internal PropertyCheckBuilder(
        IEnumerable< ICheckBuilder > checkBuilders)
        : base(
            CheckCollectionKind.All,
            checkBuilders)
    {
    }

    public override ICheck Build() => new PropertyCheck(CheckBuilders.Select(checkBuilder => checkBuilder.Build()));
}
