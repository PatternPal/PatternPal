using PatternPal.SyntaxTree.Abstractions.Entities;

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
    /// <param name="subChecks">A list of sub-<see cref="ICheck"/>s that should be checked.</param>
    /// <param name="kind">The <see cref="CheckCollectionKind"/> to use for the sub-<see cref="ICheck"/>s.</param>
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
        _matchedEntities.Add(castNode);

        // Return the result.
        return new NodeCheckResult
               {
                   ChildrenCheckResults = subCheckResults,
                   FeedbackMessage = GetFeedbackMessage(castNode),
                   CollectionKind = _kind,
                   Priority = Priority
               };
    }

    /// <summary>
    /// Run the given <paramref name="subCheck"/> on the given <paramref name="castNode"/>.
    /// </summary>
    /// <param name="ctx">The current <see cref="RecognizerContext"/>.</param>
    /// <param name="castNode">The <see cref="INode"/> to run the <paramref name="subCheck"></param> on.</param>
    /// <param name="subCheck">The <see cref="ICheck"/> to run.</param>
    /// <returns>The <see cref="ICheckResult"/> of the <paramref name="subCheck"/>.</returns>
    private ICheckResult RunCheck(
        RecognizerContext ctx,
        TNode castNode,
        ICheck subCheck)
    {
        // Store the current sub-check. This is used for the InvalidSubCheckException.
        _currentSubCheck = subCheck;

        switch (subCheck)
        {
            // These don't require any special handling.
            case ClassCheck:
            case CheckCollection:
            case ModifierCheck:
            case RelationCheck:
            case ParameterCheck:
                return subCheck.Check(
                    ctx,
                    castNode);

            // These checks can match multiple entities, the results are wrapped in a
            // NodeCheckResult.
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
                return new NotCheckResult
                       {
                           FeedbackMessage = string.Empty,
                           NestedResult = RunCheck(
                               ctx,
                               castNode,
                               notCheck.NestedCheck),
                           Priority = notCheck.Priority,
                       };

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
    /// Run the given <paramref name="nodeCheck"/> on the given <paramref name="nodes"/>.
    /// </summary>
    /// <param name="ctx">The current <see cref="RecognizerContext"/>.</param>
    /// <param name="nodes">The <see cref="INode"/>s to run the <see cref="nodeCheck"/> on.</param>
    /// <param name="nodeCheck">The <see cref="ICheck"/> to run.</param>
    /// <returns>The <see cref="ICheckResult"/> of the <paramref name="nodeCheck"/>.</returns>
    private static ICheckResult RunCheckWithMultipleMatches< T >(
        RecognizerContext ctx,
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

        // TODO: Do we want to create a dedicated result type here (to indicate that these results originated from one check)?
        // Return the result.
        return new NodeCheckResult
               {
                   ChildrenCheckResults = results,
                   CollectionKind = CheckCollectionKind.All,
                   FeedbackMessage = string.Empty,
                   Priority = nodeCheck.Priority,
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
