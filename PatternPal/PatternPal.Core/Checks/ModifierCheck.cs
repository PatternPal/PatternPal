namespace PatternPal.Core.Checks;

internal class ModifierCheck : ICheck
{
    private readonly List< IModifier > _modifiers;
    private readonly List< IModifier > _notModifiers;

    public ModifierCheck(
        List< IModifier > modifiers,
        List< IModifier > notModifiers)
    {
        _modifiers = modifiers;
        _notModifiers = notModifiers;
    }

    bool ICheck.Check(
        INode node)
    {
        if (node is not IModified modified)
        {
            throw new ArgumentException(
                $"Node must implement {typeof( IModified )}",
                nameof( node ));
        }

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

        foreach (IModifier notModifier in _notModifiers)
        {
            if (modifiers.Contains(notModifier))
            {
                return false;
            }
        }

        return true;
    }
}
