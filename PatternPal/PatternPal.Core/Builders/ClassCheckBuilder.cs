namespace PatternPal.Core.Builders;

internal class ClassCheckBuilder : CheckCollectionBuilder
{
    internal ClassCheckBuilder(Priority priority,
        IEnumerable< ICheckBuilder > checkBuilders)
        : base(priority,
            CheckCollectionKind.All,
            checkBuilders)
    {
    }

    public override ICheck Build() => new ClassCheck(Priority, CheckBuilders.Select(checkBuilder => checkBuilder.Build()));
}
