namespace PatternPal.Core.Builders;

internal class MethodCheckBuilder : ICheckBuilder
{
    private ModifierCheckBuilder ? _modifierCheckBuilder;

    ICheck ICheckBuilder.Build() => new MethodCheck
                                    {
                                        ModifierCheck = (ModifierCheck ?)_modifierCheckBuilder?.Build(),
                                    };

    internal MethodCheckBuilder Modifiers(
        ModifierCheckBuilder modifierCheckBuilder)
    {
        if (_modifierCheckBuilder is not null)
        {
            throw new ArgumentException(
                "Modifier check builder is already assigned",
                nameof( modifierCheckBuilder ));
        }

        _modifierCheckBuilder = modifierCheckBuilder;

        return this;
    }

    internal MethodCheckBuilder Modifiers(
        Action< ModifierCheckBuilder > builderAction)
    {
        _modifierCheckBuilder ??= new ModifierCheckBuilder();
        builderAction(_modifierCheckBuilder);
        return this;
    }

    internal MethodCheckBuilder ReturnType()
    {
        return this;
    }
}
