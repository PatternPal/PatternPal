namespace PatternPal.Core.Checks;

/// <summary>
/// Placeholder for a not-<see cref="ICheck"/>. The actual implementation lives in <see cref="NodeCheck{TNode}"/>.
/// </summary>
internal class NotCheck : CheckBase
{
    private readonly Score _perfectScore;

    /// <summary>
    /// The nested <see cref="ICheck"/> which should not pass.
    /// </summary>
    internal ICheck NestedCheck { get; }

    /// <summary>
    /// A <see cref="NotCheck"/> does not add any dependencies on top of the ones its <see cref="NestedCheck"/> has.
    /// Thus its amount of dependencies is equal to the amount of dependencies its <see cref="NestedCheck"/> has.
    /// </summary>
    public override int DependencyCount => NestedCheck.DependencyCount;

    /// <inheritdoc />
    public override Score PerfectScore(
        IDictionary< ICheck, ICheckResult > resultsByCheck,
        ICheckResult result) => _perfectScore;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotCheck"/> class.
    /// </summary>
    /// <param name="priority"><see cref="Priority"/> of the check.</param>
    /// <param name="check"><see cref="ICheck"/> which should not pass.</param>
    internal NotCheck(
        Priority priority,
        string ? requirement,
        ICheck check)
        : base(
            priority,
            requirement)
    {
        NestedCheck = check;
        _perfectScore = Score.CreateScore(
            Priority,
            true);
    }

    /// <inheritdoc />
    public override ICheckResult Check(
        IRecognizerContext ctx,
        INode node)
    {
        throw new UnreachableException();
    }
}
