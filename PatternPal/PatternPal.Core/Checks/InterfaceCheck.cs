using PatternPal.SyntaxTree.Abstractions.Entities;

namespace PatternPal.Core.Checks;

/// <summary>
/// <see cref="ICheck"/> implementation for <see cref="IInterface"/> entities.
/// </summary>
internal class InterfaceCheck : NodeCheck< IInterface >
{
    /// <inheritdoc cref="NodeCheck{TNode}"/>
    internal InterfaceCheck(
        Priority priority,
        IEnumerable< ICheck > checks)
        : base(
            priority,
            checks)
    {
    }

    /// <inheritdoc />
    protected override string GetFeedbackMessage(
        IInterface node) => $"Found interface '{node}'";
}
