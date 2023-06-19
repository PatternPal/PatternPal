namespace PatternPal.Core.Checks;

/// <summary>
/// <see cref="ICheck"/> implementation for <see cref="IField"/> entities.
/// </summary>
internal class FieldCheck : NodeCheck< IField >
{
    /// <inheritdoc cref="NodeCheck{TNode}"/>
    internal FieldCheck(
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
    /// <returns>The <see cref="IEntity"/> which represents the type of the <see cref="IField"/>.</returns>
    protected override IEntity GetType4TypeCheck(
        IRecognizerContext ctx,
        ICheck check,
        IField node) => ctx.Graph.Relations.GetEntityByName(node.GetFieldType())!;

    /// <inheritdoc />
    protected override string GetFeedbackMessage(
        IField node) => $"Found field '{node}'";
}
