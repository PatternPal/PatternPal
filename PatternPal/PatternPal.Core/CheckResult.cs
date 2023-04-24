namespace PatternPal.Core;

/// <summary>
/// Represents the result of a check, which is the result of a <see cref="Recognizers.IRecognizer"/>.
/// </summary>
internal interface ICheckResult
{
    //Explenation and context of the result of the check.
    string FeedbackMessage { get; init; }
    //Priority of the checked check.
    Priority Priority { get; init; }
}

/// <summary>
/// Represents the result of a check which is not a collection of other checks, like <see cref="ModifierCheck"/>, and <see cref="UsesCheck"/>.
/// </summary>
internal class LeafCheckResult : ICheckResult
{
    //Explenation and context of the result of the check.
    public required string FeedbackMessage { get; init; }
    //Priority of the checked check.
    public required Priority Priority { get; init; }
    //Whether the check succeeded or failed.
    public required bool Correct { get; init; }
}

/// <summary>
/// Represents the result of a check which is a collection of other checks, like <see cref="ClassCheck"/>, and <see cref="FieldCheck"/>.
/// </summary>
internal class NodeCheckResult : ICheckResult
{
    //Explenation and context of the result of the check.
    public required string FeedbackMessage { get; init; }
    //Priority of the checked check.
    public required Priority Priority { get; init; }
    //Collection of the results of the childchecks, like the MethodCheck inside a ClassCheck
    public required IList< ICheckResult > ChildrenCheckResults { get; init; }
}
