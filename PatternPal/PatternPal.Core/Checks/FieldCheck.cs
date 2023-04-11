namespace PatternPal.Core.Checks;

internal class FieldCheck : ICheck
{
    private readonly IEnumerable<ICheck> _checks;
    internal FieldCheck(
        IEnumerable<ICheck> checks)
    {
        _checks = checks;
    }

    public bool Check(RecognizerContext ctx, INode node)
    {
        if(node is not IField field) return false;

        foreach (ICheck check in _checks) 
        {
            if (!check.Check(ctx, node)) return false;
        }

        Console.WriteLine($"Got field {field}");

        return true;
    }
}
