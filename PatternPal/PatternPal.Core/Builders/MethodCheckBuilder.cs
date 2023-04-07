namespace PatternPal.Core.Builders;

internal class MethodCheckBuilder : CheckCollectionBuilder
{
    internal MethodCheckBuilder()
        : base(CheckCollectionKind.All)
    {
    }

    internal MethodCheckBuilder Modifiers(
        Action< ModifierCheckBuilder > builderAction)
    {
        ModifierCheckBuilder modifierCheckBuilder = new();
        CheckBuilders.Add(modifierCheckBuilder);
        builderAction(modifierCheckBuilder);
        return this;
    }

    internal MethodCheckBuilder Not(
        ICheckBuilder checkBuilder)
    {
        CheckBuilders.Add(new NotCheckBuilder(checkBuilder));
        return this;
    }

    internal MethodCheckBuilder ReturnType(
        GetCurrentEntity getCurrentEntity)
    {
        return this;
    }
}
