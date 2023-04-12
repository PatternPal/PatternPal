namespace PatternPal.Core.Checks;

internal class PropertyCheck : ICheck
{
    private readonly IEnumerable< ICheck > _checks;

    public PropertyCheck(
        IEnumerable< ICheck > checks)
    {
        _checks = checks;
    }

    public bool Check(
        RecognizerContext ctx,
        INode node)
    {
        return false;
    }
}
