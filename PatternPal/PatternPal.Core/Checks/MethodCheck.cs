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

    public CheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        if (node is not IMethod method)
        {
            throw new NotImplementedException("Method Check was incorrect");
        }

        ctx.ParentCheck = this;

        foreach (ICheck check in _checks)
        {
            if (!check.Check(
                ctx,
                node).getCorrectness())
            {
                throw new NotImplementedException("Method Check was incorrect");
            }
        }

        Console.WriteLine($"Got method '{method}'");
        MatchedEntity = method;
        throw new NotImplementedException("Method Check was correct");
    }
}
