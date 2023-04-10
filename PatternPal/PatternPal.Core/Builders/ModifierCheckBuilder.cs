namespace PatternPal.Core.Builders;

internal class ModifierCheckBuilder : ICheckBuilder
{
    private readonly List< IModifier > _modifiers;

    internal ModifierCheckBuilder(
        IEnumerable< IModifier > modifiers)
    {
        _modifiers = new List< IModifier >(modifiers);
    }

    public ICheck Build() => new ModifierCheck(_modifiers);
}
