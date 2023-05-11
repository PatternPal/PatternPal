namespace PatternPal.Core.Checks;

/// <summary>
/// <see cref="ICheck"/> implementation for <see cref="IConstructor"/> entities.
/// </summary>
internal class ConstructorCheck : NodeCheck< IConstructor >
{
    /// <inheritdoc />
    internal ConstructorCheck(
        Priority priority,
        IEnumerable< ICheck > checks)
        : base(
            priority,
            checks)
    {
    }

    /// <inheritdoc path="//summary|//param" />
    /// <returns>The <see cref="IEntity"/> which represents the type constructed by the <see cref="IConstructor"/>.</returns>
    protected override IEntity GetType4TypeCheck(
        IRecognizerContext ctx,
        IConstructor node) => node.GetParent();

    /// <inheritdoc />
    protected override string GetFeedbackMessage(
        IConstructor node) => $"Found constructor: {node}.";
}
