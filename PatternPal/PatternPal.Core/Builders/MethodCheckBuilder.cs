namespace PatternPal.Core.Builders;

internal class MethodCheckBuilder : ICheckBuilder
{
    private ModifierCheckBuilder ? _modifierCheckBuilder;

    ICheck ICheckBuilder.Build() => new MethodCheck
                                    {
                                        ModifierCheck = (ModifierCheck ?)_modifierCheckBuilder?.Build(),
                                    };

    internal MethodCheckBuilder Modifiers(
        Action< ModifierCheckBuilder > builderAction)
    {
        return ICheckBuilder.BuildCheck(
            this,
            ref _modifierCheckBuilder,
            builderAction);
    }

    internal MethodCheckBuilder ReturnType(
        GetCurrentEntity getCurrentEntity)
    {
        return this;
    }
}
