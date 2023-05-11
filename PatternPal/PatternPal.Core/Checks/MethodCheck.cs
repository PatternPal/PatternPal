using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Members;

namespace PatternPal.Core.Checks;

/// <summary>
/// <see cref="ICheck"/> implementation for <see cref="IMethod"/> entities.
/// </summary>
internal class MethodCheck : NodeCheck< IMethod >
{
    /// <inheritdoc cref="NodeCheck{TNode}"/>
    internal MethodCheck(
        Priority priority,
        IEnumerable< ICheck > checks)
        : base(
            priority,
            checks)
    {
    }

    /// <inheritdoc path="//summary|//param" />
    /// <returns>The <see cref="IEntity"/> which represents the return type of the <see cref="IMethod"/>.</returns>
    protected override IEntity GetType4TypeCheck(
        RecognizerContext ctx,
        IMethod node) => ctx.Graph.Relations.GetEntityByName(node.GetReturnType())!;

    /// <inheritdoc />
    protected override string GetFeedbackMessage(
        IMethod node) => $"Found method: {node}.";
}
