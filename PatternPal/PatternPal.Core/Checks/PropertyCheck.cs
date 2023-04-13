namespace PatternPal.Core.Checks;

internal class PropertyCheck : ICheck
{
    private readonly IEnumerable< ICheck > _checks;

    public PropertyCheck(
        IEnumerable< ICheck > checks)
    {
        _checks = checks;
    }

    public ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        throw new NotImplementedException("Property Check was not implemented");
    }
}
