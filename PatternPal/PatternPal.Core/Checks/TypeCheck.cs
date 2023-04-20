namespace PatternPal.Core.Checks;

/// <summary>
/// Checks for the type of an entity. Depending on the <see cref="Func{TResult}"/> provided, this
/// can be the type of a field, or the return type of a method, ect. It should match with the
/// current <see cref="INode"/> being checked.
/// </summary>
internal class TypeCheck : CheckBase
{
    // Used to get the node to compare against.
    private readonly OneOf< Func< INode >, GetCurrentEntity > _getNode;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeCheck"/> class.
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="getNode">A functor to get the node to compare against.</param>
    internal TypeCheck(
        Priority priority,
        OneOf< Func< INode >, GetCurrentEntity > getNode)
        : base(priority)
    {
        _getNode = getNode;
    }

    /// <summary>
    /// This check is marked as correct when the given <paramref name="node"/> matches the node
    /// provided by either of the functors which are set using the constructors.
    /// </summary>
    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        // Get the node to match against.
        INode nodeToMatch = _getNode.Match(
            getNode => getNode(),
            getCurrentEntity => getCurrentEntity(ctx));

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
