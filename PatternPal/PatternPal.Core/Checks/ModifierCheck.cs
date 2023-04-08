namespace PatternPal.Core.Checks;

internal class ModifierCheck : ICheck
{
    private readonly List< IModifier > _modifiers;

    public ModifierCheck(
        List< IModifier > modifiers)
    {
        _modifiers = modifiers;
    }

    public bool Check(
        RecognizerContext ctx,
        INode node)
    {
        if (node is not IModified modified)
        {
            return false;
        }

        Console.WriteLine("got modifier check");

        // TODO: This method is terribly inefficient, rewrite Modifiers to be a flags enum and just
        // check if a bit is set or not.
        // TODO: Report which modifiers are missing/incorrect.
        List< IModifier > modifiers = modified.GetModifiers().ToList();

        foreach (IModifier modifier in _modifiers)
        {
            if (!modifiers.Contains(modifier))
            {
                return false;
            }
        }

        return true;
    }
}
