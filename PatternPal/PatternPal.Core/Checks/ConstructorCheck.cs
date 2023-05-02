namespace PatternPal.Core.Checks;

/// <summary>
/// Checks for a constructor of an entity, depending on the list of <see cref="_checks"/> provided.
/// The checks performed can be a collection of <see cref="ModifierCheck"/>s,
/// <see cref="ParameterCheck"/>s, <see cref="RelationCheck"/>s, etc.
/// </summary>
internal class ConstructorCheck : CheckBase
{
    //A list of checks needed to perform on the method.
    private readonly IEnumerable<ICheck> _checks;

    //A list of all found instances adhering to _checks
    internal List<INode> MatchedEntities { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstructorCheck"/> class.
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="checks">A list of checks needed to perform on the method.</param>
    internal ConstructorCheck(Priority priority, 
        IEnumerable<ICheck> checks) : base(priority)
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
        IConstructor constructorEntity = CheckHelper.ConvertNodeElseThrow<IConstructor>(node);

        IList<ICheckResult> subCheckResults = new List<ICheckResult>();
        foreach (ICheck check in _checks)
        {
            switch (check)
            {
                case ModifierCheck modifierCheck:
                {
                    subCheckResults.Add(modifierCheck.Check(ctx, constructorEntity));
                    break;
                }
                case TypeCheck typeCheck:
                {
                    subCheckResults.Add(typeCheck.Check(ctx, constructorEntity.GetParent()));
                    break;
                }
                case NotCheck notCheck:
                {
                    subCheckResults.Add(
                        notCheck.Check(
                            ctx,
                            constructorEntity));
                    break;
                }
                case RelationCheck relationCheck:
                {
                    subCheckResults.Add(relationCheck.Check(ctx, constructorEntity));
                    break;
                }
                case ParameterCheck parameterCheck:
                {
                    // TODO getParameters voor constructor.
                    //throw new NotImplementedException();
                    IEntity methodParamaterTypes = 
                        ctx.Graph.Relations.GetEntityByName(
                            constructorEntity.GetReturnType());
                    subCheckResults.Add(
                        parameterCheck.Check(
                            ctx, 
                            constructorEntity));
                        break;
                }
                default:
                    throw CheckHelper.InvalidSubCheck(
                        this,
                        check);
            }
        }

        //add the checked constructor to the list of found constructors
        MatchedEntities.Add(constructorEntity);

        return new NodeCheckResult { ChildrenCheckResults = subCheckResults, Priority = Priority, FeedbackMessage = $"Found constructor: {constructorEntity}." };
    }
}
