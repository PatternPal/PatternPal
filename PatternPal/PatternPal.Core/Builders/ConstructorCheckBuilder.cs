namespace PatternPal.Core.Builders;

internal class ConstructorCheckBuilder : CheckCollectionBuilder
{
    private readonly ConstructorCheck _check;

    public ConstructorCheckBuilder(Priority priority,
        IEnumerable<ICheckBuilder> checkBuilders)
        : base(priority,
            CheckCollectionKind.All,
            checkBuilders)
    {
        _check = new ConstructorCheck(Priority, CheckBuilders.Select(checkBuilder => checkBuilder.Build()));
    }

    public override ICheck Build() => _check;

    //internal Func<INode> Result => () => _check.MatchedEntity!; TODO
}
