﻿namespace PatternPal.Core.Checks;

/// <summary>
/// <see cref="ICheck"/> implementation for <see cref="IMethod"/> entities.
/// </summary>
internal class MethodCheck : NodeCheck< IMethod >
{
    /// <inheritdoc cref="NodeCheck{TNode}"/>
    internal MethodCheck(
        Priority priority,
        string ? requirement,
        IEnumerable< ICheck > checks)
        : base(
            priority,
            requirement,
            checks)
    {
    }

    /// <inheritdoc path="//summary|//param" />
    /// <returns>The <see cref="IEntity"/> which represents the return type of the <see cref="IMethod"/>.</returns>
    protected override IEntity GetType4TypeCheck(
        IRecognizerContext ctx,
        ICheck check,
        IMethod node) => ctx.Graph.Relations.GetEntityByName(node.GetReturnType())!;

    /// <inheritdoc />
    protected override string GetFeedbackMessage(
        IMethod node) => $"Found method: {node}.";
}
