namespace PatternPal.Core.Builders;

internal class ModifierCheckBuilder : CheckBuilderBase
{
    private readonly List< IModifier > _modifiers;

    internal ModifierCheckBuilder(Priority priority,
        IEnumerable< IModifier > modifiers) : base(priority)
    {
        _modifiers = new List< IModifier >(modifiers);
        Priority = priority;
    }

    public override ICheck Build() => new ModifierCheck(Priority, _modifiers);
}
