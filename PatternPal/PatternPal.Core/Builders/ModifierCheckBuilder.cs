namespace PatternPal.Core.Builders;

internal class ModifierCheckBuilder : ICheckBuilder
{
    private readonly List< IModifier > _modifiers;

    public ModifierCheckBuilder()
    {
        _modifiers = new List< IModifier >();
    }

    public ICheck Build() => new ModifierCheck(_modifiers);

    internal ModifierCheckBuilder Modifiers(
        params IModifier[ ] modifiers)
    {
        _modifiers.AddRange(modifiers);
        return this;
    }
}
