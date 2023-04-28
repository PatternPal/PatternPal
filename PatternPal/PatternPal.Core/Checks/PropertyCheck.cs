namespace PatternPal.Core.Checks;

/// <summary>
/// Checks for a property of an entity, depending on the list of <see cref="_checks"/> provided.
/// The checks performed can be a collection of <see cref="TypeCheck"/>s, <see cref="ModifierCheck"/>s,
/// <see cref="ParameterCheck"/>s, <see cref="UsesCheck"/>s, etc.
/// </summary>
internal class PropertyCheck : CheckBase
{
    //A list of checks needed to perform on the method.
    private readonly IEnumerable< ICheck > _checks;

    //A list of all found instances
    internal List<INode> MatchedEntities { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyCheck"/> class.
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="checks">A list of checks needed to perform on the property.</param>
    public PropertyCheck(Priority priority,
        IEnumerable< ICheck > checks) : base(priority)
    {
        MatchedEntities = new List<INode>();
        _checks = checks;
    }

    /// <summary>
    /// This method executes all the given checks on the <paramref name="node"/>
    /// </summary>
    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        IProperty propertyEntity = CheckHelper.ConvertNodeElseThrow<IProperty>(node);

        IList<ICheckResult> subCheckResults = new List<ICheckResult>();
        foreach (ICheck check in _checks)
        {
            switch (check)
            {
                case ModifierCheck modifierCheck:
                {
                    subCheckResults.Add(modifierCheck.Check(ctx, propertyEntity));
                    break;
                }
                case TypeCheck typeCheck:
                { 
                    //TODO return type needs to be obtained from the propertyEntity
                    throw new NotImplementedException();
                }
                case NotCheck notCheck:
                    {
                        subCheckResults.Add(
                            notCheck.Check(
                                ctx,
                                propertyEntity));
                        break;
                    }
                case UsesCheck usesCheck:
                {
                    subCheckResults.Add(usesCheck.Check(ctx, propertyEntity));
                    break;
                }
                case ParameterCheck parameterCheck:
                {
                    throw new NotImplementedException();
                }
                default:
                    throw CheckHelper.InvalidSubCheck(
                        this,
                        check);
            }
        }

        //add the checked method to the list of found methods
        MatchedEntities.Add(propertyEntity);

        return new NodeCheckResult { ChildrenCheckResults = subCheckResults, Priority = Priority, FeedbackMessage = $"Found property: {propertyEntity}." };
    }
}
