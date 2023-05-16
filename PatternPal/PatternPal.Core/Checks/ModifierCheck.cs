namespace PatternPal.Core.Checks;

/// <summary>
/// Checks for the modifiers of an entity. Depending on the <see cref="List{T}"/> of <see cref="IModified"/> provided.
/// It should match with the modifiers of the current <see cref="INode"/> being checked.
/// </summary>
internal class ModifierCheck : CheckBase
{
    // all modifiers the node should have
    private readonly IEnumerable< IModifier > _modifiers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModifierCheck"/> class. 
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="modifiers">A list of modifiers the node should have</param>
    public ModifierCheck(
        Priority priority,
        IEnumerable< IModifier > modifiers)
        : base(priority)
    {
        _modifiers = modifiers;
    }

    /// <summary>
    /// This check is marked as correct when the modifiers of the given <paramref name="node"/>
    /// match the provided modifiers. 
    /// </summary>
    public override ICheckResult Check(
        IRecognizerContext ctx,
        INode node)
    {
        // cast node to IModified
        IModified modified = CheckHelper.ConvertNodeElseThrow< IModified >(node);

        // all modifiers the node really has
        List< IModifier > modifiers = modified.GetModifiers().ToList();

        // TODO: This method is terribly inefficient, rewrite Modifiers to be a flags enum and just
        // check if a bit is set or not.

        foreach (IModifier modifier in _modifiers)
        {
            if (!modifiers.Contains(modifier)) // if the needed modifier is not one of the node's modifiers
            {
                return new LeafCheckResult
                       {
                           FeedbackMessage = $"The node {node} does not have the {modifier} modifier.",
                           Correct = false,
                           Priority = Priority
                       };
            }
        }

        return new LeafCheckResult
               {
                   FeedbackMessage = "Modifiers correctly implemented.",
                   Correct = true,
                   Priority = Priority
               };
    }
}
