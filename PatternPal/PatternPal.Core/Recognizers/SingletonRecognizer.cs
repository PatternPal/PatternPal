namespace PatternPal.Core.Recognizers;

internal class SingletonRecognizer
{
    internal void Create(
        IRootCheckBuilder rootCheckBuilder)
    {
        rootCheckBuilder
            .Class(
                classBuilder => classBuilder
                    .Method(
                        methodBuilder => methodBuilder
                                         .Modifiers(
                                             modifierBuilder => modifierBuilder
                                                 .NotModifiers(Modifiers.Static)
                                         )
                                         .ReturnType(ICheckBuilder.GetCurrentEntity)
                    )
            );
    }
}
