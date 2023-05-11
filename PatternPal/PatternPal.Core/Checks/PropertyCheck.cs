namespace PatternPal.Core.Checks;

/// <summary>
/// <see cref="ICheck"/> implementation for <see cref="IProperty"/> entities.
/// </summary>
internal class PropertyCheck : NodeCheck< IProperty >
{
    /// <inheritdoc cref="NodeCheck{TNode}"/>
    public PropertyCheck(
        Priority priority,
        IEnumerable< ICheck > checks)
        : base(
            priority,
            checks)
    {
    }

    /// <inheritdoc />
    protected override string GetFeedbackMessage(
        IProperty node) => $"Found property: {node}.";
}
