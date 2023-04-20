namespace PatternPal.Core.Builders;

internal class MethodCheckBuilder : CheckCollectionBuilder
{
    private readonly MethodCheck _check;

    internal MethodCheckBuilder(Priority priority,
        IEnumerable< ICheckBuilder > checkBuilders)
        : base(priority,
            CheckCollectionKind.All,
            checkBuilders)
    {
        _check = new MethodCheck(Priority, CheckBuilders.Select(checkBuilder => checkBuilder.Build()));
    }

    public override ICheck Build() => _check;

    internal Func< List<INode> > Result => () => _check.MatchedEntities!;
}
