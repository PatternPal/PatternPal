namespace PatternPal.Core.Checks;

/// <summary>
/// <see cref="ICheck"/> implementation for <see cref="IConstructor"/> entities.
/// </summary>
internal class ConstructorCheck : NodeCheck< IConstructor >
{
    /// <inheritdoc />
    internal ConstructorCheck(
        Priority priority,
        string ? requirement,
        IEnumerable< ICheck > checks)
        : base(
            priority,
            requirement,
            checks)
    {
    }

    /// <inheritdoc />
    protected override string GetFeedbackMessage(
        IConstructor node) => $"Found constructor: {node}.";
}
