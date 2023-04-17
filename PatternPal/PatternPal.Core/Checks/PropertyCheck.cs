namespace PatternPal.Core.Checks;

internal class PropertyCheck : CheckBase
{
    private readonly IEnumerable< ICheck > _checks;

    public PropertyCheck(Priority priority,
        IEnumerable< ICheck > checks) : base(priority)
    {
        _checks = checks;
    }

    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        throw new NotImplementedException("Property Check was not implemented");
    }
}
