namespace PatternPal.Core.Checks;

internal class FieldCheck : ICheck
{
    private readonly IEnumerable<ICheck> _checks;
    internal FieldCheck(
        IEnumerable<ICheck> checks)
    {
        _checks = checks;
    }

    public ICheckResult Check(RecognizerContext ctx, INode node)
    {
        if(node is not IField field) throw new NotImplementedException("Field Check was incorrect");

        foreach (ICheck check in _checks) 
        {
            if (!check.Check(ctx, node).Correctness) throw new NotImplementedException("Field Check was incorrect");
        }

        Console.WriteLine($"Got field {field}");

        throw new NotImplementedException("Field Check was correct");
    }
}
