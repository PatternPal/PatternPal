namespace PatternPal.Core.Checks;

/// <summary>
/// Base class for <see cref="ICheck"/>s which can have sub-<see cref="ICheck"/>s.
/// </summary>
/// <typeparam name="TNode">The <see cref="INode"/> type which this <see cref="ICheck"/> supports.</typeparam>
internal class NodeCheck< TNode > : CheckBase
    where TNode : INode
{
    // The sub-checks of the current check.
    private readonly IEnumerable< ICheck > _subChecks;

    // The kind of this collection of checks.
    private readonly CheckCollectionKind _kind;

    /// <summary>
    /// Gets a <see cref="Func{TResult}"/> which returns a <see cref="List{T}"/> of <see cref="IEntity"/>s matched by this <see cref="ICheck"/>.
    /// </summary>
    /// <returns>A <see cref="List{T}"/> of matched <see cref="IEntity"/>s.</returns>
    public override Func< List< INode > > Result => () => _matchedEntities
                                                          ?? throw new ArgumentNullException(
                                                              nameof( _matchedEntities ),
                                                              $"'{this}' is not yet evaluated, make sure to evaluate this check before you try to access it results!");

    // The current sub-check being checked.
    private ICheck ? _currentSubCheck;

    // The entities matched by this check. This list is set when the check is evaluated, before then
    // it is null. When the list is set but empty, no entities were matched by this check.
    private List< INode > ? _matchedEntities;

    // The dependency count, declared as nullable so we can check whether we have calculated it
    // already.
    private int ? _dependencyCount;

    /// <summary>
    /// The dependencies to other <see cref="INode"/>s this check has.
    /// While calculating the dependencies, it calculates the dependencies of its <see cref="_subChecks"/>.
    /// </summary>
    public override int DependencyCount
    {
        get
        {
            // Only compute the dependency count if we haven't done so already.
            if (_dependencyCount is null)
            {
                int dependencyCount = 0;
                foreach (ICheck subCheck in _subChecks)
                {
                    dependencyCount += subCheck.DependencyCount;
                }

                _dependencyCount = dependencyCount;
            }

            return _dependencyCount.Value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NodeCheck{TNode}"/> class.
    /// </summary>
    /// <param name="priority"><see cref="Priority"/> of the check.</param>
    /// <param name="subChecks">A list of sub-<see cref="ICheck"/>s that should be checked.</param>
    /// <param name="kind">The <see cref="CheckCollectionKind"/> to use for the sub-<see cref="ICheck"/>s.</param>
    internal NodeCheck(
        Priority priority,
        IEnumerable< ICheck > subChecks,
        CheckCollectionKind kind = CheckCollectionKind.All)
        : base(priority)
    {
        _subChecks = subChecks;
        _kind = kind;
    }

    /// <inheritdoc />
    public override ICheckResult Check(
        IRecognizerContext ctx,
        INode node)
    {
        // Verify that the node can be handled by this check.
        TNode castNode = CheckHelper.ConvertNodeElseThrow< TNode >(node);

        // Run the sub-checks.
        IList< ICheckResult > subCheckResults = new List< ICheckResult >();
        foreach (ICheck subCheck in _subChecks)
        {
            subCheckResults.Add(
                RunCheck(
                    ctx,
                    castNode,
                    subCheck));
        }

        // Store the matched entity.
        _matchedEntities ??= new List< INode >();
        if (!castNode.IsPlaceholder)
        {
            _matchedEntities.Add(castNode);
        }

        // Return the result.
        return new NodeCheckResult
               {
                   ChildrenCheckResults = subCheckResults,
                   FeedbackMessage = GetFeedbackMessage(castNode),
                   CollectionKind = _kind,
                   Priority = Priority,
                   DependencyCount = DependencyCount,
                   MatchedNode = !castNode.IsPlaceholder
                       ? castNode
                       : null,
                   Check = this,
               };
    }

    /// <summary>
    /// Run the given <paramref name="subCheck"/> on the given <paramref name="castNode"/>.
    /// </summary>
    /// <param name="oldCtx">The current <see cref="IRecognizerContext"/>.</param>
    /// <param name="castNode">The <see cref="INode"/> to run the <paramref name="subCheck"/> on.</param>
    /// <param name="subCheck">The <see cref="ICheck"/> to run.</param>
    /// <returns>The <see cref="ICheckResult"/> of the <paramref name="subCheck"/>.</returns>
    private ICheckResult RunCheck(
        IRecognizerContext oldCtx,
        TNode castNode,
        ICheck subCheck)
    {
        // Store the current sub-check. This is used for the InvalidSubCheckException.
        _currentSubCheck = subCheck;

        // Create the new recognizer context.
        IRecognizerContext ctx = IRecognizerContext.From(
            oldCtx,
            castNode,
            this);

        switch (subCheck)
        {
            // These don't require any special handling.
            case ModifierCheck:
            case RelationCheck:
            case ParameterCheck:
            case NodeCheck< INode >:
                return subCheck.Check(
                    ctx,
                    castNode);

            // These checks can match multiple entities, the results are wrapped in a
            // NodeCheckResult.
            case ClassCheck classCheck:
                ThrowIfNested(
                    ctx,
                    subCheck);
                return RunCheckWithMultipleMatches(
                    ctx,
                    ctx.Graph.GetAll().Values.OfType< IClass >(),
                    classCheck);
            case InterfaceCheck interfaceCheck:
                ThrowIfNested(
                    ctx,
                    subCheck);
                return RunCheckWithMultipleMatches(
                    ctx,
                    ctx.Graph.GetAll().Values.OfType< IInterface >(),
                    interfaceCheck);
            case MethodCheck methodCheck:
                return RunCheckWithMultipleMatches(
                    ctx,
                    CheckHelper.ConvertNodeElseThrow< IEntity >(castNode).GetMethods(),
                    methodCheck);
            case FieldCheck fieldCheck:
                return RunCheckWithMultipleMatches(
                    ctx,
                    CheckHelper.ConvertNodeElseThrow< IClass >(castNode).GetFields(),
                    fieldCheck);
            case ConstructorCheck constructorCheck:
                return RunCheckWithMultipleMatches(
                    ctx,
                    CheckHelper.ConvertNodeElseThrow< IClass >(castNode).GetConstructors(),
                    constructorCheck);
            case PropertyCheck propertyCheck:
                return RunCheckWithMultipleMatches(
                    ctx,
                    CheckHelper.ConvertNodeElseThrow< IEntity >(castNode).GetProperties(),
                    propertyCheck);

            // Call this method recursively with the check wrapped by the NotCheck. This is
            // necessary because otherwise the wrapped check won't receive the correct entities.
            case NotCheck notCheck:
            {
                ICheckResult nestedResult = RunCheck(
                    ctx,
                    castNode,
                    notCheck.NestedCheck);
                return new NotCheckResult
                       {
                           FeedbackMessage = "Executing NOT-check",
                           NestedResult = nestedResult,
                           Priority = notCheck.Priority,
                           DependencyCount = notCheck.DependencyCount,
                           MatchedNode = nestedResult.MatchedNode,
                           Check = notCheck,
                       };
            }

            // The type to pass to the TypeCheck depends on the implementation in derived classes of
            // this class.
            case TypeCheck typeCheck:
                return typeCheck.Check(
                    ctx,
                    GetType4TypeCheck(
                        ctx,
                        castNode));

            // Ensure all checks are handled.
            default:
                throw CheckHelper.InvalidSubCheck(
                    this,
                    subCheck);
        }
    }

    /// <summary>
    /// Throws an <see cref="InvalidSubCheckException"/> if <paramref name="subCheck"/> is not a root <see cref="ICheck"/>.
    /// </summary>
    /// <param name="ctx">The current <see cref="IRecognizerContext"/>.</param>
    /// <param name="subCheck">The current <see cref="ICheck"/>.</param>
    /// <exception cref="InvalidSubCheckException">Thrown if <paramref name="subCheck"/> is not a root <see cref="ICheck"/>.</exception>
    private void ThrowIfNested(
        IRecognizerContext ctx,
        ICheck subCheck)
    {
        // Being nested inside an 'Any' or 'All' check is allowed, as this doesn't influence which
        // entities are passed to the check.
        if (ctx.ParentCheck.GetType() != typeof( NodeCheck< INode > ))
        {
            throw CheckHelper.InvalidSubCheck(
                this,
                subCheck);
        }
    }

    /// <summary>
    /// Run the given <paramref name="nodeCheck"/> on the given <paramref name="nodes"/>.
    /// </summary>
    /// <param name="ctx">The current <see cref="IRecognizerContext"/>.</param>
    /// <param name="nodes">The <see cref="INode"/>s to run the <see cref="nodeCheck"/> on.</param>
    /// <param name="nodeCheck">The <see cref="ICheck"/> to run.</param>
    /// <returns>The <see cref="ICheckResult"/> of the <paramref name="nodeCheck"/>.</returns>
    private static ICheckResult RunCheckWithMultipleMatches< T >(
        IRecognizerContext ctx,
        IEnumerable< T > nodes,
        NodeCheck< T > nodeCheck)
        where T : INode
    {
        // Run the check on the nodes.
        IList< ICheckResult > results = new List< ICheckResult >();
        foreach (T node in nodes)
        {
            results.Add(
                nodeCheck.Check(
                    ctx,
                    node));
        }

        // Prevent null-ref when `nodeCheck` is not evaluated because there are no nodes to match
        // on. This should not trigger the evaluation exception, so assign an empty list to prevent
        // that.
        if (results.Count == 0)
        {
            MarkCheckAsSeen(nodeCheck);
        }

        // Return the result.
        return new NodeCheckResult
               {
                   ChildrenCheckResults = results,
                   CollectionKind = CheckCollectionKind.Any,
                   NodeCheckCollectionWrapper = true,
                   FeedbackMessage = string.Empty,
                   Priority = nodeCheck.Priority,
                   DependencyCount = nodeCheck.DependencyCount,
                   MatchedNode = null,
                   Check = nodeCheck,
               };
    }

    /// <summary>
    /// Marks the given <paramref name="nodeCheck"/> and any sub-<see cref="ICheck"/>s as seen.
    /// </summary>
    private static void MarkCheckAsSeen< T >(
        NodeCheck< T > nodeCheck)
        where T : INode
    {
        nodeCheck._matchedEntities ??= new List< INode >();

        foreach (ICheck subCheck in nodeCheck._subChecks)
        {
            MarkCheckAsSeenImpl(subCheck);
        }

        void MarkCheckAsSeenImpl(
            ICheck subCheck)
        {
            switch (subCheck)
            {
                // These checks don't have any sub-checks, so we don't need to do anything.
                case ModifierCheck:
                case RelationCheck:
                case ParameterCheck:
                case TypeCheck:
                    return;

                // These checks can have sub-checks, so we also need to mark the sub-checks as
                // seen.
                case NodeCheck< INode > nestedNodeCheck:
                    MarkCheckAsSeen(nestedNodeCheck);
                    break;
                case ClassCheck classCheck:
                    MarkCheckAsSeen(classCheck);
                    break;
                case InterfaceCheck interfaceCheck:
                    MarkCheckAsSeen(interfaceCheck);
                    break;
                case MethodCheck methodCheck:
                    MarkCheckAsSeen(methodCheck);
                    break;
                case FieldCheck fieldCheck:
                    MarkCheckAsSeen(fieldCheck);
                    break;
                case ConstructorCheck constructorCheck:
                    MarkCheckAsSeen(constructorCheck);
                    break;
                case PropertyCheck propertyCheck:
                    MarkCheckAsSeen(propertyCheck);
                    break;

                // Mark the nested check of the NotCheck as seen.
                case NotCheck notCheck:
                    MarkCheckAsSeenImpl(notCheck.NestedCheck);
                    break;

                default:
                    throw CheckHelper.InvalidSubCheck(
                        nodeCheck,
                        subCheck);
            }
        }
    }

    /// <summary>
    /// Gets the <see cref="IEntity"/> to pass to a <see cref="TypeCheck"/>.
    /// </summary>
    /// <param name="ctx">The current <see cref="IRecognizerContext"/>.</param>
    /// <param name="node">The <see cref="INode"/> to be checked.</param>
    /// <returns>The <see cref="IEntity"/> to pass to the <see cref="TypeCheck"/>.</returns>
    protected virtual IEntity GetType4TypeCheck(
        IRecognizerContext ctx,
        TNode node)
    {
        // If the the current check is wrapped in an Any/All check, we need to walk up the checks
        // tree until we find a parent which can be used to get the type.
        IRecognizerContext ? currentContext = ctx;
        while (currentContext is {ParentCheck: NodeCheck< INode >})
        {
            currentContext = currentContext.PreviousContext;
        }

        if (currentContext != null)
        {
            // NOTE: Only the checks which implement `GetType4TypeCheck` should be matched here.
            switch (currentContext.ParentCheck)
            {
                case FieldCheck fieldCheck:
                    return fieldCheck.GetType4TypeCheck(
                        currentContext,
                        (IField)node);
                case MethodCheck methodCheck:
                    return methodCheck.GetType4TypeCheck(
                        currentContext,
                        (IMethod)node);
                case PropertyCheck propertyCheck:
                    return propertyCheck.GetType4TypeCheck(
                        currentContext,
                        (IProperty)node);
            }
        }

        throw new InvalidSubCheckException(
            this,
            _currentSubCheck!);
    }

    /// <summary>
    /// Gets the feedback message for the current <see cref="ICheck"/>.
    /// </summary>
    /// <param name="node">The <see cref="INode"/> on which the check is run.</param>
    /// <returns>The feedback message.</returns>
    protected virtual string GetFeedbackMessage(
        TNode node) => $"Found the required checks for: {node}.";
}
