namespace PatternPal.Core.Recognizers;

internal class SingletonRecognizer
{
    internal ICheck Create()
    {
        ICheckBuilder modifierCheckBuilder =
            new ModifierCheckBuilder()
                .Modifiers(
                    Modifiers.Private,
                    Modifiers.Abstract)
                .NotModifiers(Modifiers.Public);

        return modifierCheckBuilder.Build();
    }
}
