namespace PatternPal.Core.Checks;

/// <summary>
/// <see cref="ICheck"/> implementation for <see cref="IClass"/> entities.
/// </summary>
public class ClassCheck : NodeCheck< IClass >
{
    /// <inheritdoc cref="NodeCheck{TNode}"/>
    internal ClassCheck(
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
        IClass node) => $"Found class '{node}'";
}
