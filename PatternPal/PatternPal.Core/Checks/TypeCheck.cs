namespace PatternPal.Core.Checks;

/// <summary>
/// Checks for the type of an entity. Depending on the <see cref="Func{TResult}"/> provided, this
/// can be the type of a field, or the return type of a method, ect. It should match with the
/// current <see cref="INode"/> being checked.
/// </summary>
internal class TypeCheck : CheckBase
{
    // Used to get the node to compare against.
    private readonly OneOf< ICheck, GetCurrentEntity > _getNode;

    /// <summary>
    /// A <see cref="TypeCheck"/> is dependent on the result of <see cref="_getNode"/>.
    /// </summary>
    public override int DependencyCount => 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeCheck"/> class.
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="getNode">A functor to get the node to compare against.</param>
    internal TypeCheck(
        Priority priority,
        OneOf< ICheck, GetCurrentEntity > getNode)
        : base(priority)
    {
        _getNode = getNode;
    }

    /// <summary>
    /// This check is marked as correct when the given <paramref name="node"/> matches the node
    /// provided by either of the functors which are set using the constructors.
    /// </summary>
    public override ICheckResult Check(
        IRecognizerContext ctx,
        INode node)
    {
        // Get the node to match against.
        return _getNode.Match< ICheckResult >(
            relatedCheck =>
            {
                List< ICheckResult > subResults = new();
                foreach (INode getNode in relatedCheck.Result())
                {
                    bool isMatch = node == getNode;
                    subResults.Add(
                        new LeafCheckResult
                        {
                            Priority = Priority,
                            Correct = isMatch,
                            FeedbackMessage = isMatch
                                ? $"Node '{node}' has correct type"
                                : $"Node '{node}' has incorrect type, expected '{getNode}'",
                            DependencyCount = DependencyCount,
                            MatchedNode = getNode,
                            Check = this,
                            RelatedCheck = relatedCheck
                        });
                }

                return new NodeCheckResult
                       {
                           Priority = Priority,
                           ChildrenCheckResults = subResults,
                           FeedbackMessage = $"Found node '{node}'",
                           DependencyCount = DependencyCount,
                           MatchedNode = node,
                           Check = this,
                           NodeCheckCollectionWrapper = true,
                           CollectionKind = CheckCollectionKind.Any
                       };
            },
            getCurrentEntity =>
            {
                // Construct and return the check result.
                INode nodeToMatch = getCurrentEntity(ctx);
                bool isMatch = node == nodeToMatch;
                List<ICheckResult> subResults = new()
                {
                    new LeafCheckResult
                    {
                        Priority = Priority,
                        Correct = isMatch,
                        FeedbackMessage = isMatch
                            ? $"Node '{node}' has correct type"
                            : $"Node '{node}' has incorrect type, expected '{nodeToMatch}'",
                        DependencyCount = DependencyCount,
                        MatchedNode = nodeToMatch, //TODO is this right or should this be node?
                        Check = this,
                        RelatedCheck = ctx.EntityCheck
                    }
                };
                return new NodeCheckResult
                {
                    Priority = Priority,
                    ChildrenCheckResults = subResults,
                    FeedbackMessage = $"Found node '{node}'",
                    DependencyCount = DependencyCount,
                    MatchedNode = node,
                    Check = this,
                    NodeCheckCollectionWrapper = true,
                    CollectionKind = CheckCollectionKind.Any
                };
            });
    }
}
