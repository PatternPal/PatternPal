namespace PatternPal.Core.Builders;

internal interface IMethodCheckBuilder : ICheckBuilder
{
    IMethodCheckBuilder Modifiers(
        IModifierCheckBuilder modifierCheckBuilder);

    IMethodCheckBuilder Modifiers(
        Action< IModifierCheckBuilder > builderAction);

    IMethodCheckBuilder ReturnType(
        GetCurrentEntity getCurrentEntity);
}

internal class MethodCheckBuilder : IMethodCheckBuilder
{
    private IModifierCheckBuilder ? _modifierCheckBuilder;

    ICheck ICheckBuilder.Build() => new MethodCheck
                                    {
                                        ModifierCheck = (ModifierCheck ?)_modifierCheckBuilder?.Build(),
                                    };

    IMethodCheckBuilder IMethodCheckBuilder.Modifiers(
        IModifierCheckBuilder modifierCheckBuilder)
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

    IMethodCheckBuilder IMethodCheckBuilder.Modifiers(
        Action< IModifierCheckBuilder > builderAction)
    {
        _modifierCheckBuilder ??= new ModifierCheckBuilder();
        builderAction(_modifierCheckBuilder);
        return this;
    }

    IMethodCheckBuilder IMethodCheckBuilder.ReturnType(
        GetCurrentEntity getCurrentEntity)
    {
        return this;
    }
}
