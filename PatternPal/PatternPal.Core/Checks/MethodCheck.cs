namespace PatternPal.Core.Checks;

internal class MethodCheck : ICheck
{
    private readonly IEnumerable< ICheck > _checks;

    // TODO CV: Handle multiple matches (e.g. a method check for a public method may match many methods).
    internal IMethod ? MatchedEntity { get; private set; }

    internal MethodCheck(
        IEnumerable< ICheck > checks)
    {
        _checks = checks;
    }

    public bool Check(
        RecognizerContext ctx,
        INode node)
    {
        if (node is not IMethod method)
        {
            return false;
        }

        ctx.ParentCheck = this;

        foreach (ICheck check in _checks)
        {
            if (!check.Check(
                ctx,
                node))
            {
                return false;
            }
        }

        Console.WriteLine($"Got method '{method}'");
        MatchedEntity = method;
        return true;
    }
}
