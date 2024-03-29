﻿namespace PatternPal.Core.Checks;

/// <summary>
/// <see cref="ICheck"/> implementation for <see cref="IProperty"/> entities.
/// </summary>
internal class PropertyCheck : NodeCheck< IProperty >
{
    /// <inheritdoc cref="NodeCheck{TNode}"/>
    public PropertyCheck(
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
    /// <returns>The <see cref="IEntity"/> which represents the type of the <see cref="IProperty"/>.</returns>
    protected override IEntity GetType4TypeCheck(
        IRecognizerContext ctx,
        ICheck check,
        IProperty node) => ctx.Graph.Relations.GetEntityByName(node.GetPropertyType())!;

    /// <inheritdoc />
    protected override string GetFeedbackMessage(
        IProperty node) => $"Found property: {node}.";
}
