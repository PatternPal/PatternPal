namespace PatternPal.Core.Checks;

internal class CheckCollection : CheckBase
{
    private readonly CheckCollectionKind _checkCollectionKind;
    private readonly IList< ICheck > _checks;

    internal CheckCollection(Priority priority,
        CheckCollectionKind checkCollectionKind,
        IList< ICheck > checks) : base(priority)
    {
        _checkCollectionKind = checkCollectionKind;
        _checks = checks;
    }

    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        throw new NotImplementedException();
    }
}
