namespace PatternPal.Core.Checks;

/// <summary>
/// Base class for <see cref="ICheck"/>s which can have sub-<see cref="ICheck"/>s.
/// </summary>
/// <typeparam name="TNode">The <see cref="INode"/> type which this <see cref="ICheck"/> supports.</typeparam>
internal abstract class NodeCheck< TNode > : CheckBase
    where TNode : INode
{
    // The sub-checks of the current check.
    private readonly IEnumerable< ICheck > _subChecks;

    // The kind of this collection of checks.
    private readonly CheckCollectionKind _kind;

    // The entities matched by this check.
    private readonly List< INode > _matchedEntities;

    // The current sub-check being checked.
    private ICheck ? _currentSubCheck;

    /// <summary>
    /// Gets a <see cref="Func{TResult}"/> which returns a <see cref="List{T}"/> of <see cref="IEntity"/>s matched by this <see cref="ICheck"/>.
    /// </summary>
    /// <returns>A <see cref="List{T}"/> of matched <see cref="IEntity"/>s.</returns>
    internal Func< List< INode > > Result => () => _matchedEntities;

    /// <summary>
    /// Initializes a new instance of the <see cref="NodeCheck{TNode}"/> class.
    /// </summary>
    /// <param name="priority"><see cref="Priority"/> of the check.</param>
    /// <param name="checks">A list of sub-<see cref="ICheck"/>s that should be checked.</param>
    protected NodeCheck(
        Priority priority,
        IEnumerable< ICheck > subChecks,
        CheckCollectionKind kind = CheckCollectionKind.All)
        : base(priority)
    {
        _subChecks = subChecks;
        _kind = kind;
        _matchedEntities = new List< INode >();
    }

    /// <inheritdoc />
    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        // Verify that the node can be handled by this check.
        TNode castNode = CheckHelper.ConvertNodeElseThrow< TNode >(node);

        IList< ICheckResult > subCheckResults = new List< ICheckResult >();
        foreach (ICheck subCheck in _subChecks)
        {
            _currentSubCheck = subCheck;
            switch (subCheck)
            {
                case ClassCheck classCheck:
                {
                    subCheckResults.Add(
                        classCheck.Check(
                            ctx,
                            castNode));
                    break;
                }
                case CheckCollection checkCollection:
                {
                    subCheckResults.Add(
                        checkCollection.Check(
                            ctx,
                            castNode));
                    break;
                }
                case ModifierCheck modifierCheck:
                {
                    subCheckResults.Add(
                        modifierCheck.Check(
                            ctx,
                            castNode));
                    break;
                }
                case MethodCheck methodCheck:
                {
                    IEntity entity = CheckHelper.ConvertNodeElseThrow< IEntity >(node);
                    foreach (IMethod method in entity.GetMethods())
                    {
                        subCheckResults.Add(
                            methodCheck.Check(
                                ctx,
                                method));
                    }
                    break;
                }
                case FieldCheck fieldCheck:
                {
                    IClass classEntity = CheckHelper.ConvertNodeElseThrow< IClass >(node);
                    foreach (IField field in classEntity.GetFields())
                    {
                        subCheckResults.Add(
                            fieldCheck.Check(
                                ctx,
                                field));
                    }
                    break;
                }
                case ConstructorCheck constructorCheck:
                {
                    IClass classEntity = CheckHelper.ConvertNodeElseThrow< IClass >(node);
                    foreach (IConstructor constructor in classEntity.GetConstructors())
                    {
                        subCheckResults.Add(
                            constructorCheck.Check(
                                ctx,
                                constructor));
                    }
                    break;
                }
                case PropertyCheck propertyCheck:
                {
                    IClass classEntity = CheckHelper.ConvertNodeElseThrow< IClass >(node);
                    foreach (IProperty property in classEntity.GetProperties())
                    {
                        subCheckResults.Add(
                            propertyCheck.Check(
                                ctx,
                                property));
                    }
                    break;
                }
                case NotCheck notCheck:
                {
                    throw new NotImplementedException();
                }
                case TypeCheck typeCheck:
                {
                    subCheckResults.Add(
                        typeCheck.Check(
                            ctx,
                            GetType4TypeCheck(
                                ctx,
                                castNode)));
                    break;
                }
                case RelationCheck relationCheck:
                {
                    subCheckResults.Add(
                        relationCheck.Check(
                            ctx,
                            castNode));
                    break;
                }
                case ParameterCheck parameterCheck:
                {
                    subCheckResults.Add(
                        parameterCheck.Check(
                            ctx,
                            castNode));
                    break;
                }
                default:
                    throw CheckHelper.InvalidSubCheck(
                        this,
                        subCheck);
            }
        }

        _matchedEntities.Add(castNode);

        return new NodeCheckResult
               {
                   ChildrenCheckResults = subCheckResults,
                   FeedbackMessage = GetFeedbackMessage(castNode),
                   CollectionKind = _kind,
                   Priority = Priority
               };
    }

    /// <summary>
    /// Gets the <see cref="IEntity"/> to pass to a <see cref="TypeCheck"/>.
    /// </summary>
    /// <param name="ctx">The current <see cref="RecognizerContext"/>.</param>
    /// <param name="node">The <see cref="INode"/> to be checked.</param>
    /// <returns>The <see cref="IEntity"/> to pass to the <see cref="TypeCheck"/>.</returns>
    protected virtual IEntity GetType4TypeCheck(
        RecognizerContext ctx,
        TNode node)
    {
        throw new InvalidSubCheckException(
            this,
            _currentSubCheck!);
    }

    /// <summary>
    /// Gets the feedback message for the current <see cref="ICheck"/>.
    /// </summary>
    /// <param name="node">The <see cref="INode"/> on which the check is run.</param>
    /// <returns>The feedback message.</returns>
    protected abstract string GetFeedbackMessage(
        TNode node);
}
