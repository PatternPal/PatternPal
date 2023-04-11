namespace PatternPal.Core.Builders;

internal class ClassCheckBuilder : CheckCollectionBuilder
{
    internal ClassCheckBuilder(
        IEnumerable< ICheckBuilder > checkBuilders)
        : base(
            CheckCollectionKind.All,
            checkBuilders)
    {
    }

    public override ICheck Build() => new ClassCheck(CheckBuilders.Select(checkBuilder => checkBuilder.Build()));
}
