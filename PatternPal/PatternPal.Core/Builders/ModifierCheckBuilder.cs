namespace PatternPal.Core.Builders;

internal class ModifierCheckBuilder : ICheckBuilder
{
    private readonly List< IModifier > _modifiers;
    private readonly List< IModifier > _notModifiers;

    public ModifierCheckBuilder()
    {
        _modifiers = new List< IModifier >();
        _notModifiers = new List< IModifier >();
    }

    public ICheck Build() => new ModifierCheck(
        _modifiers,
        _notModifiers);

    internal ModifierCheckBuilder Modifiers(
        params IModifier[ ] modifiers)
    {
        _modifiers.AddRange(modifiers);
        return this;
    }

    internal ModifierCheckBuilder NotModifiers(
        params IModifier[ ] notModifiers)
    {
        _notModifiers.AddRange(notModifiers);
        return this;
    }
}
