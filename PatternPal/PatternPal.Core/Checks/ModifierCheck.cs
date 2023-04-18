namespace PatternPal.Core.Checks;

internal class ModifierCheck : CheckBase
{
    //all modifiers the node should have
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
        //cast node to IModified
        IModified modified = CheckHelper.ConvertNodeElseThrow<IModified>(node);

        //all modifiers the node really has
        List< IModifier > modifiers = modified.GetModifiers().ToList();

        // TODO: This method is terribly inefficient, rewrite Modifiers to be a flags enum and just
        // check if a bit is set or not.

        foreach (IModifier modifier in _modifiers)
        {
            if (!modifiers.Contains(modifier)) //if the needed modifier is not one of the node's modifiers
            {
                return new LeafCheckResult{FeedbackMessage = $"The node {node} does not have the {modifier} modifier.", Correctness = false, Priority = Priority.Low};
            }
        }

        return new LeafCheckResult{FeedbackMessage = "Modifiers correctly implemented.", Correctness = true, Priority = Priority.Low};
    }
}
