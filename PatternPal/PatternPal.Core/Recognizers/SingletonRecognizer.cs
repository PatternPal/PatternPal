namespace PatternPal.Core.Recognizers;

internal class SingletonRecognizer
{
    internal void Create(
        RootCheckBuilder rootCheckBuilder)
    {
        rootCheckBuilder
            .Class(
                classBuilder => classBuilder
                    .Any(
                        new MethodCheckBuilder()
                            .Modifiers(
                                modifierBuilder => modifierBuilder
                                                   .Modifiers(Modifiers.Static)
                                                   .NotModifiers(Modifiers.Private)
                            )
                            .ReturnType(ICheckBuilder.GetCurrentEntity)
                    )
            );
    }
}
