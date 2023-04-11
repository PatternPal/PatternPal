namespace PatternPal.Core.Checks;

internal class NotCheck : ICheck
{
    private readonly ICheck _check;

    internal NotCheck(
        ICheck check)
    {
        _check = check;
    }

    bool ICheck.Check(
        RecognizerContext ctx,
        INode node)
    {
        return !_check.Check(
            ctx,
            node);
    }
}
