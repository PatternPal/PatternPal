namespace PatternPal.Core;

internal interface ICheckResult
{
    string FeedbackMessage { get; init; }
    Priority Priority { get; init; }
}

internal class LeafCheckResult : ICheckResult
{
    public required string FeedbackMessage { get; init; }
    public required Priority Priority { get; init; }
    public required bool Correct { get; init; }
}

internal class NodeCheckResult : ICheckResult
{
    public required string FeedbackMessage { get; init; }
    public required Priority Priority { get; init; }
    public required IList< ICheckResult > ChildrenCheckResults { get; init; }
}
