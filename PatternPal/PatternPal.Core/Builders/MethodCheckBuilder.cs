namespace PatternPal.Core.Builders;

internal class MethodCheckBuilder : CheckCollectionBuilder
{
    private readonly MethodCheck _check;

    internal MethodCheckBuilder(
        IEnumerable< ICheckBuilder > checkBuilders)
        : base(
            CheckCollectionKind.All,
            checkBuilders)
    {
        _check = new MethodCheck(CheckBuilders.Select(checkBuilder => checkBuilder.Build()));
    }

    public override ICheck Build() => _check;

    internal Func< INode > Result => () => _check.MatchedEntity!;
}
