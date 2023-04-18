namespace PatternPal.Core.Builders;
internal class FieldCheckBuilder : CheckCollectionBuilder
{
    private readonly FieldCheck _check;

    internal FieldCheckBuilder(Priority priority, IEnumerable<ICheckBuilder> checkBuilders) : base(priority, CheckCollectionKind.All, checkBuilders)
    {
        _check = new FieldCheck(Priority, CheckBuilders.Select(checkBuilder => checkBuilder.Build()));
    }

    public override ICheck Build() => _check;

    //internal Func<INode> Result => () => _check.MatchedEntity!; TODO
}
