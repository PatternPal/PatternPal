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

    /// <inheritdoc />
    protected override string GetFeedbackMessage(
        IConstructor node) => $"Found constructor: {node}.";
}
