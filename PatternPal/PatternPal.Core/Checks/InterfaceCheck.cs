namespace PatternPal.Core.Checks;

/// <summary>
/// <see cref="ICheck"/> implementation for <see cref="IInterface"/> entities.
/// </summary>
public class InterfaceCheck : NodeCheck< IInterface >
{
    /// <inheritdoc cref="NodeCheck{TNode}"/>
    internal InterfaceCheck(
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
        IInterface node) => $"Found interface '{node}'";
}
