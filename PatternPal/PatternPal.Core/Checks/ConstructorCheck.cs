namespace PatternPal.Core.Checks;

internal class ConstructorCheck : ICheck
{
    private readonly IEnumerable<ICheck> _checks;

    // TODO CV: Handle multiple matches (e.g. a method check for a public method may match many methods).
    internal IMethod? MatchedEntity { get; private set; }

    internal ConstructorCheck(
        IEnumerable<ICheck> checks)
    {
        _checks = checks;
    }

    public bool Check(
        RecognizerContext ctx,
        INode node)
    {
        if (node is not IConstructor constructor) return false;

        foreach (ICheck check in _checks)
        {
            if (!check.Check(ctx, node)) return false;
        }

        Console.WriteLine($"Got Constructor '{constructor}'");
        
        return true;
    }
}
