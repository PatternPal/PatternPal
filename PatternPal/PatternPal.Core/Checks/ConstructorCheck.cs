namespace PatternPal.Core.Checks;

internal class ConstructorCheck : CheckBase
{
    private readonly IEnumerable<ICheck> _checks;

    // TODO CV: Handle multiple matches (e.g. a method check for a public method may match many methods).
    internal IMethod? MatchedEntity { get; private set; }

    internal ConstructorCheck(Priority priority, 
        IEnumerable<ICheck> checks) : base(priority)
    {
        _checks = checks;
    }

    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        throw new NotImplementedException("Constructor Check was correct");
    }
}
