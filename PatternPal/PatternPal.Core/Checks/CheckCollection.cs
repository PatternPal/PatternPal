namespace PatternPal.Core.Checks;

/// <summary>
/// A collection of checks. The <see cref="CheckCollectionKind"/> determines when it is considered
/// correct based on the results of the sub-<see cref="ICheck"/>s.
/// </summary>
internal class CheckCollection : NodeCheck< INode >
{
    /// <inheritdoc cref="NodeCheck{TNode}"/>
    /// <param name="checkCollectionKind"><see cref="CheckCollectionKind"/> of this collection.</param>
    internal CheckCollection(
        Priority priority,
        CheckCollectionKind checkCollectionKind,
        IList< ICheck > checks)
        : base(
            priority,
            checks,
            checkCollectionKind)
    {
    }

    /// <inheritdoc />
    protected override string GetFeedbackMessage(
        INode node) => $"Found the required checks for: {node}.";
}
