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
        if (node is not IConstructor constructor)
        {
            throw new NotImplementedException("Constructor check failed");
        }

        foreach (ICheck check in _checks)
        {
            if (!check.Check(ctx, node).Correctness) throw new NotImplementedException("Constructor Check was incorrect");
        }

        Console.WriteLine($"Got Constructor '{constructor}'");

        throw new NotImplementedException("Constructor Check was correct");
    }
}
