namespace PatternPal.Core.Builders;

internal class MethodCheckBuilder : CheckCollectionBuilder
{
    internal MethodCheckBuilder()
        : base(CheckCollectionKind.All)
    {
    }

    internal MethodCheckBuilder Not(
        Action< MethodCheckBuilder > builderAction)
    {
        MethodCheckBuilder nestedBuilder = new();
        builderAction(nestedBuilder);
        CheckBuilders.Add(new NotCheckBuilder(nestedBuilder));
        return this;
    }

    internal MethodCheckBuilder Modifiers(
        params IModifier[ ] modifiers)
    {
        CheckBuilders.Add(new ModifierCheckBuilder(modifiers));
        return this;
    }

    internal MethodCheckBuilder Modifiers(
        Action< ModifierCheckBuilder > builderAction)
    {
        ModifierCheckBuilder modifierCheckBuilder = new();
        CheckBuilders.Add(modifierCheckBuilder);
        builderAction(modifierCheckBuilder);
        return this;
    }

    internal MethodCheckBuilder ReturnType(
        GetCurrentEntity getCurrentEntity)
    {
        return this;
    }
}
