namespace PatternPal.Core.Builders;
internal class FieldCheckBuilder : CheckCollectionBuilder
{
    private readonly FieldCheck _check;

    internal FieldCheckBuilder(CheckCollectionKind checkCollectionKind) : base(checkCollectionKind)
    {
        _check = new FieldCheck(CheckBuilders.Select(checkBuilder => checkBuilder.Build()));
    }

    internal FieldCheckBuilder(IEnumerable<ICheckBuilder> checkBuilders) : base(CheckCollectionKind.All, checkBuilders)
    {
        _check = new FieldCheck(CheckBuilders.Select(checkBuilder => checkBuilder.Build()));
    }

    public override ICheck Build() => _check;

    //internal Func<INode> Result => () => _check.MatchedEntity!; TODO
}
