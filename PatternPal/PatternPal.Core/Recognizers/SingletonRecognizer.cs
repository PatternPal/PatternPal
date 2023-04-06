namespace PatternPal.Core.Recognizers;

internal class SingletonRecognizer
{
    internal ICheck Create()
    {
        MethodCheckBuilder methodCheckBuilder =
            new MethodCheckBuilder()
                .Modifiers(
                    builder => builder
                        .NotModifiers(Modifiers.Static))
                // TODO: Func to get current entity (from context or something like that)
                .ReturnType();

        return ((ICheckBuilder)methodCheckBuilder).Build();
    }
}
