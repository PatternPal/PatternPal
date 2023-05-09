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

    /// <inheritdoc />
    protected override IEntity GetType4TypeCheck(
        RecognizerContext ctx,
        IMethod node)
    {
        return ctx.Graph.Relations.GetEntityByName(node.GetReturnType())!;
    }

    /// <inheritdoc />
    protected override string GetFeedbackMessage(
        IMethod node) => $"Found method: {node}.";
}
