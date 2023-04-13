namespace PatternPal.Core.Builders;

internal class ConstructorCheckBuilder : CheckCollectionBuilder
{
    private readonly ConstructorCheck _check;

    internal ConstructorCheckBuilder()
        : base(CheckCollectionKind.All)
    {
        _check = new ConstructorCheck(CheckBuilders.Select(checkBuilder => checkBuilder.Build()));
    }

    public ConstructorCheckBuilder(
        IEnumerable<ICheckBuilder> checkBuilders)
        : base(
            CheckCollectionKind.All,
            checkBuilders)
    {
        _check = new ConstructorCheck(CheckBuilders.Select(checkBuilder => checkBuilder.Build()));
    }

    public override ICheck Build() => _check;

    //internal Func<INode> Result => () => _check.MatchedEntity!; TODO
}
