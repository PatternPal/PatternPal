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
                                            Modifiers.Static
                                        )
                                        .Not(
                                            builder => builder
                                                .Modifiers(Modifiers.Private)
                                        )
                                        .ReturnType(ICheckBuilder.GetCurrentEntity)
                                )
                                .Not(
                                    builder => builder
                                        .Method(
                                            methodBuilder => methodBuilder
                                                .Modifiers(Modifiers.Public)
                                        )
                                )
            );
    }
}
