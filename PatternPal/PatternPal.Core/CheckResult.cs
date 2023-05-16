namespace PatternPal.Core;

/// <summary>
/// Represents the result of a check, which is the result of a <see cref="Recognizers.IRecognizer"/>.
/// </summary>
[JsonDerivedType(typeof( LeafCheckResult ))]
[JsonDerivedType(typeof( NotCheckResult ))]
[JsonDerivedType(typeof( NodeCheckResult ))]
public interface ICheckResult
{
    /// <summary>
    /// Explanation and context of the result of the check.
    /// </summary>
    string FeedbackMessage { get; init; }

    /// <summary>
    /// Priority of the checked check.
    /// </summary>
    Priority Priority { get; init; }

    /// <summary>
    /// The number of other <see cref="INode"/>s the <see cref="ICheck"/> depends on.
    /// </summary>
    int DependencyCount { get; init; }

    /// <summary>
    /// The <see cref="INode"/> on which the <see cref="ICheck"/> was run.
    /// </summary>
    INode ? MatchedNode { get; init; }
}

/// <summary>
/// Represents the result of a check which is not a collection of other checks, like <see cref="ModifierCheck"/>, and <see cref="RelationCheck"/>.
/// </summary>
internal class LeafCheckResult : ICheckResult
{
    /// <inheritdoc />
    public required string FeedbackMessage { get; init; }

    /// <inheritdoc />
    public required Priority Priority { get; init; }

    /// <summary>
    /// Whether the check succeeded or failed.
    /// </summary>
    public required bool Correct { get; init; }

    /// <inheritdoc />
    public required int DependencyCount { get; init; }

    /// <inheritdoc />
    public required INode ? MatchedNode { get; init; }
}

/// <summary>
/// Represents the result of a <see cref="NotCheck"/>.
/// </summary>
internal class NotCheckResult : ICheckResult
{
    /// <inheritdoc />
    public required string FeedbackMessage { get; init; }

    /// <inheritdoc />
    public required Priority Priority { get; init; }

    /// <summary>
    /// The <see cref="ICheckResult"/> of the <see cref="ICheck"/> nested in the <see cref="NotCheck"/>.
    /// </summary>
    public required ICheckResult NestedResult { get; init; }

    /// <inheritdoc />
    public required int DependencyCount { get; init; }

    /// <inheritdoc />
    public required INode ? MatchedNode { get; init; }
}

/// <summary>
/// Represents the result of a check which is a collection of other checks, like <see cref="ClassCheck"/>, and <see cref="FieldCheck"/>.
/// </summary>
internal class NodeCheckResult : ICheckResult
{
    /// <inheritdoc />
    public required string FeedbackMessage { get; init; }

    /// <inheritdoc />
    public required Priority Priority { get; init; }

    /// <summary>
    /// Collection of the results of the childchecks, like the MethodCheck inside a ClassCheck
    /// </summary>
    public required IList< ICheckResult > ChildrenCheckResults { get; init; }

    /// <summary>
    /// Behavior of the collection of sub-<see cref="ICheck"/>s.
    /// </summary>
    public CheckCollectionKind CollectionKind { get; init; } = CheckCollectionKind.All;

    /// <inheritdoc />
    public required int DependencyCount { get; init; }

    /// <inheritdoc />
    public required INode ? MatchedNode { get; init; }

    /// <summary>
    /// <see langword="true"/> if the sub-<see cref="ICheckResult"/>s are the result of one
    /// <see cref="NodeCheck{TNode}"/>.
    /// </summary>
    /// <remarks>
    /// For example, a <see cref="MethodCheck"/> is run on multiple <see cref="IMethod"/>s. For each
    /// <see cref="IMethod"/> the <see cref="MethodCheck"/> is run on, a
    /// <see cref="NodeCheckResult"/> is created. All these results are wrapped in a
    /// <see cref="NodeCheckResult"/>, with <see cref="NodeCheckCollectionWrapper"/> set
    /// to <see langword="true"/>.
    /// </remarks>
    public bool NodeCheckCollectionWrapper { get; set; }
}
