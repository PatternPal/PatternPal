using PatternPal.SyntaxTree.Abstractions.Entities;

namespace PatternPal.Core.Checks;

/// <summary>
/// <see cref="ICheck"/> implementation for <see cref="IClass"/> entities.
/// </summary>
internal class ClassCheck : NodeCheck< IClass >
{
    /// <inheritdoc cref="NodeCheck{TNode}"/>
    internal ClassCheck(
        Priority priority,
        IEnumerable< ICheck > checks)
        : base(
            priority,
            checks)
    {
    }

    /// <inheritdoc />
    protected override string GetFeedbackMessage(
        IClass node) => $"Found class '{node}'";
}
