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
        if(node is not IField) return false;

        foreach (ICheck check in _checks) 
        {
            if (!check.Check(ctx, node)) return false;
        }

        Console.WriteLine("field found");

        return true;
    }
}
