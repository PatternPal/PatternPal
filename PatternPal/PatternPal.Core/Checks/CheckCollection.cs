namespace PatternPal.Core.Checks;

/// <summary>
/// A collection of checks. The <see cref="CheckCollectionKind"/> determines when it is considered
/// correct based on the results of the sub-<see cref="ICheck"/>s.
/// </summary>
internal class CheckCollection : CheckBase
{
    private readonly CheckCollectionKind _checkCollectionKind;
    private readonly IList< ICheck > _checks;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckCollection"/> class. 
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="checkCollectionKind"><see cref="CheckCollectionKind"/> of this collection.</param>
    /// <param name="checks">The sub-<see cref="ICheck"/>s of this collection.</param>
    internal CheckCollection(
        Priority priority,
        CheckCollectionKind checkCollectionKind,
        IList< ICheck > checks)
        : base(priority)
    {
        _checkCollectionKind = checkCollectionKind;
        _checks = checks;
    }

    /// <inheritdoc />
    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        IList< ICheckResult > subCheckResults = new List< ICheckResult >();
        foreach (ICheck check in _checks)
        {
            subCheckResults.Add(
                check.Check(
                    ctx,
                    node));
        }

        return new NodeCheckResult
               {
                   ChildrenCheckResults = subCheckResults,
                   FeedbackMessage = $"Found the required checks for: {node}.",
                   Priority = Priority,
                   CollectionKind = _checkCollectionKind,
               };
    }
}
