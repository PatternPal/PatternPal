namespace PatternPal.Core.Checks;

/// <summary>
/// Placeholder for a not-<see cref="ICheck"/>. The actual implementation lives in <see cref="NodeCheck{TNode}"/>.
/// </summary>
internal class NotCheck : CheckBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotCheck"/> class.
    /// </summary>
    /// <param name="priority"><see cref="Priority"/> of the check.</param>
    /// <param name="check"><see cref="ICheck"/> which should not pass.</param>
    internal NotCheck(
        Priority priority,
        ICheck check)
        : base(priority)
    {
        NestedCheck = check;
    }

    /// <summary>
    /// <see cref="ICheck"/> which should not pass.
    /// </summary>
    internal ICheck NestedCheck { get; }

    /// <inheritdoc />
    public override ICheckResult Check(
        IRecognizerContext ctx,
        INode node)
    {
        throw new UnreachableException();
    }
}
