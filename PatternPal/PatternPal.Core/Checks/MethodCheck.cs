namespace PatternPal.Core.Checks;

internal class MethodCheck : CheckBase
{
    private readonly IEnumerable< ICheck > _checks;

    // TODO CV: Handle multiple matches (e.g. a method check for a public method may match many methods).
    internal IMethod ? MatchedEntity { get; private set; }

    internal MethodCheck(Priority priority,
        IEnumerable< ICheck > checks) : base(priority)
    {
        _checks = checks;
    }

    public override ICheckResult Check(
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
                node).Correctness)
            {
                throw new NotImplementedException("Method Check was incorrect");
            }
        }

        Console.WriteLine($"Got method '{method}'");
        MatchedEntity = method;

        return new NodeCheckResult{ChildrenCheckResults = new List<ICheckResult>(), Correctness = true, Priority = Priority, FeedbackMessage = "method found"};

        throw new NotImplementedException("Method Check was correct");
    }
}
