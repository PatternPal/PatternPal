namespace PatternPal.Core.Builders;

internal interface IModifierCheckBuilder : ICheckBuilder
{
    IModifierCheckBuilder Modifiers(
        params IModifier[ ] modifiers);

    IModifierCheckBuilder NotModifiers(
        params IModifier[ ] notModifiers);
}

internal class ModifierCheckBuilder : IModifierCheckBuilder
{
    private readonly List< IModifier > _modifiers;
    private readonly List< IModifier > _notModifiers;

    public ModifierCheckBuilder()
    {
        _modifiers = new List< IModifier >();
        _notModifiers = new List< IModifier >();
    }

    ICheck ICheckBuilder.Build() => new ModifierCheck(
        _modifiers,
        _notModifiers);

    IModifierCheckBuilder IModifierCheckBuilder.Modifiers(
        params IModifier[ ] modifiers)
    {
        _modifiers.AddRange(modifiers);
        return this;
    }

    IModifierCheckBuilder IModifierCheckBuilder.NotModifiers(
        params IModifier[ ] notModifiers)
    {
        _notModifiers.AddRange(notModifiers);
        return this;
    }
}
