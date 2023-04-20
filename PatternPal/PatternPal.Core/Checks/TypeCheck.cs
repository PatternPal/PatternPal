namespace PatternPal.Core.Checks;

/// <summary>
/// Checks for the type of an entity. Depending on the <see cref="Func{TResult}"/> provided, this
/// can be the type of a field, or the return type of a method, ect. It should match with the
/// current <see cref="INode"/> being checked.
/// </summary>
internal class TypeCheck : CheckBase
{
    // Can get the current entity which is being checked.
    private readonly GetCurrentEntity ? _getCurrentEntity;

    // Can get an arbitrary node.
    private readonly Func< INode > ? _getNode;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeCheck"/> class.
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="getNode">
    /// <see cref="Func{TResult}"/> which returns an arbitrary <see cref="INode"/> which is to be compared against the current <see cref="INode"/>.
    /// </param>
    /// <param name="getCurrentEntity">
    /// <see cref="GetCurrentEntity"/> which returns the current <see cref="IEntity"/> being checked, which might not be the same as the current <see cref="INode"/>.
    /// </param>
    internal TypeCheck(
        Priority priority,
        Func< INode > ? getNode,
        GetCurrentEntity ? getCurrentEntity)
        : base(priority)
    {
        _getNode = getNode;
        _getCurrentEntity = getCurrentEntity;
    }

    /// <summary>
    /// This check is marked as correct when the given <paramref name="node"/> matches the node
    /// provided by either of the functors which are set using the constructors.
    /// </summary>
    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        // TODO: Should we use OneOf here?
        // Check that exactly one of _getNode and _getCurrentEntity is set.
        if (_getNode is null
            && _getCurrentEntity is null)
        {
            throw new ArgumentException($"{nameof( _getNode )} and {nameof( _getCurrentEntity )} cannot both be null");
        }

        if (_getNode is not null
            && _getCurrentEntity is not null)
        {
            throw new ArgumentException($"{nameof( _getNode )} and {nameof( _getCurrentEntity )} cannot both be present");
        }

        // KNOWN: Only one of the functors is set, so we can check them independently and store
        // result of both, without having to worry about overwriting anything.
        INode ? nodeToMatch = null;
        if (_getNode is not null)
        {
            nodeToMatch = _getNode();
        }

        if (_getCurrentEntity is not null)
        {
            nodeToMatch = _getCurrentEntity(ctx);
        }

        // Construct and return the check result.
        bool isMatch = node == nodeToMatch;
        return new LeafCheckResult
               {
                   Priority = Priority,
                   Correctness = isMatch,
                   FeedbackMessage = isMatch
                       ? $"Node '{node}' has correct type"
                       : $"Node '{node}' has incorrect type, expected '{nodeToMatch}'",
               };
    }
}
