using PatternPal.SyntaxTree.Abstractions.Members;

namespace PatternPal.Core.Checks;

/// <summary>
/// <see cref="ICheck"/> implementation for <see cref="IField"/> entities.
/// </summary>
internal class FieldCheck : NodeCheck< IField >
{
    /// <inheritdoc cref="NodeCheck{TNode}"/>
    internal FieldCheck(
        Priority priority,
        IEnumerable< ICheck > checks)
        : base(
            priority,
            checks)
    {
    }

    /// <inheritdoc />
    protected override string GetFeedbackMessage(
        IField node) => $"Found field '{node}'";
}
