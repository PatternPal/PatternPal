namespace PatternPal.Core;

/// <summary>
/// Represents the result of a check, which is the result of a <see cref="Recognizers.IRecognizer"/>.
/// </summary>
[JsonDerivedType(typeof( LeafCheckResult ))]
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
}

/// <summary>
/// Represents the result of a check which is not a collection of other checks, like <see cref="ModifierCheck"/>, and <see cref="UsesCheck"/>.
/// </summary>
public class LeafCheckResult : ICheckResult
{
    /// <inheritdoc />
    public required string FeedbackMessage { get; init; }

    /// <inheritdoc />
    public required Priority Priority { get; init; }

    /// <summary>
    /// Whether the check succeeded or failed.
    /// </summary>
    public required bool Correct { get; init; }
}

/// <summary>
/// Represents the result of a check which is a collection of other checks, like <see cref="ClassCheck"/>, and <see cref="FieldCheck"/>.
/// </summary>
public class NodeCheckResult : ICheckResult
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
}
