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

    private Score _perfectScore;

    /// <summary>
    /// A <see cref="TypeCheck"/> is dependent on the result of <see cref="_getNode"/>.
    /// </summary>
    public override int DependencyCount => _getNode.Match(
        relatedCheck => 1 + relatedCheck.DependencyCount,
        _ => 0);

    /// <summary>
    /// Whether this <see cref="TypeCheck "/> is for the current <see cref="IEntity"/>.
    /// </summary>
    internal bool IsForCurrentEntity => _getNode.Match(
        _ => false,
        _ => true);

    /// <inheritdoc />
    public override Score PerfectScore => _perfectScore.Equals(default)
        ? _perfectScore = Score.CreateScore(
                              Priority,
                              true)
                          + _getNode.Match(
                              RelationCheck.PerfectScoreFromRelatedCheck,
                              _ => default)
        : _perfectScore;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeCheck"/> class.
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="requirement">The optional requirement which this <see cref="ICheck"/> checks.</param>
    /// <param name="getNode">A functor to get the node to compare against.</param>
    internal TypeCheck(
        Priority priority,
        string ? requirement,
        OneOf< ICheck, GetCurrentEntity > getNode)
        : base(
            priority,
            requirement)
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
        List< ICheckResult > results = _getNode.Match(
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
                return subResults;
            },
            getCurrentEntity =>
            {
                // Construct and return the check result.
                INode nodeToMatch = getCurrentEntity(ctx);
                bool isMatch = node == nodeToMatch;
                return new List< ICheckResult >
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
            });

        return new NodeCheckResult
        {
           Priority = Priority,
           ChildrenCheckResults = results,
           FeedbackMessage = $"Found node '{node}'",
           DependencyCount = DependencyCount,
           MatchedNode = node,
           Check = this,
           NodeCheckCollectionWrapper = true,
           CollectionKind = CheckCollectionKind.Any
        };
    }
}
