namespace PatternPal.Core.Checks;

internal class MethodCheck : ICheck
{
    internal ModifierCheck ? ModifierCheck { get; init; }

    public bool Check(
        INode node)
    {
        if (node is not IEntity entity)
        {
            throw new ArgumentException(
                $"Node must implement {typeof( IEntity )}",
                nameof( node ));
        }

        bool ? modifierResult = ModifierCheck?.Check(entity);

        return true;
    }
}
