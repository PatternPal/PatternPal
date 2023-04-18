namespace PatternPal.Core.Checks;

internal class ModifierCheck : CheckBase
{
    private readonly List< IModifier > _modifiers;

    public ModifierCheck(Priority priority,
        List< IModifier > modifiers) : base(priority)
    {
        _modifiers = modifiers;
    }

    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        IModified modified = CheckHelper.ConvertNodeElseThrow<IModified>(node);

        // TODO: This method is terribly inefficient, rewrite Modifiers to be a flags enum and just
        // check if a bit is set or not.
        List< IModifier > modifiers = modified.GetModifiers().ToList();

        foreach (IModifier modifier in _modifiers)
        {
            if (!modifiers.Contains(modifier))
            {
                return new LeafCheckResult{FeedbackMessage = $"The node {node} does not have the {modifier} modifier.", Correctness = false, Priority = Priority.Undefined};
            }
        }

        return new LeafCheckResult{FeedbackMessage = "Modifiers correctly implemented.", Correctness = true, Priority = Priority.Undefined};
    }
}
