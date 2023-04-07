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
        INode node)
    {
        return !_check.Check(node);
    }
}
