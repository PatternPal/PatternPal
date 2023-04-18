namespace PatternPal.Core.Checks;

internal class NotCheck : CheckBase
{
    private readonly ICheck _check;

    internal NotCheck(Priority priority,
        ICheck check) : base(priority)
    {
        _check = check;
    }

    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        //Todo: add implememtation
        // return !_check.Check( ctx, node).Correctness;
        throw new NotImplementedException("Not Check was incorrect");
    }
}
