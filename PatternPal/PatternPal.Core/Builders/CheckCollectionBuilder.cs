namespace PatternPal.Core.Builders;

enum CheckCollectionKind
{
    All,
    Any
}

internal class CheckCollectionBuilder : CheckBuilderBase
{
    private readonly CheckCollectionKind _checkCollectionKind;
    protected readonly IList< ICheckBuilder > CheckBuilders;

    internal CheckCollectionBuilder(Priority priority,
        CheckCollectionKind checkCollectionKind) : base(priority)
    {
        _checkCollectionKind = checkCollectionKind;
        CheckBuilders = new List< ICheckBuilder >();
    }

    internal CheckCollectionBuilder(Priority priority,
        CheckCollectionKind checkCollectionKind,
        IEnumerable< ICheckBuilder > checkBuilders) : base(priority)
    {
        _checkCollectionKind = checkCollectionKind;
        CheckBuilders = checkBuilders as List< ICheckBuilder > ?? new List< ICheckBuilder >(checkBuilders);
    }

    public override ICheck Build()
    {
        IList< ICheck > checks = new List< ICheck >(CheckBuilders.Count);
        foreach (ICheckBuilder checkBuilder in CheckBuilders)
        {
            checks.Add(checkBuilder.Build());
        }

        return new CheckCollection(Priority,
            _checkCollectionKind,
            checks);
    }
    
}
