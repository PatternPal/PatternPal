namespace PatternPal.Core.Checks;

internal class NotCheck : ICheck
{
    private readonly ICheck _check;

    internal NotCheck(
        ICheck check)
    {
        _check = check;
    }

    ICheckResult ICheck.Check(
        RecognizerContext ctx,
        INode node)
    {
        //Todo: add implememtation
        // return !_check.Check( ctx, node).Correctness;
        throw new NotImplementedException("Not Check was incorrect");
    }
}
