using static PatternPal.Core.Recognizers.CheckBuilder;

namespace PatternPal.Core.Recognizers;

internal class SingletonRecognizer
{
    internal void Create(
        RootCheckBuilder rootCheckBuilder)
    {
        rootCheckBuilder
            .Class(
                c => c
                     .Any(
                         a => a
                             .Method(
                                 m => m
                                      .Modifiers(Modifiers.Static)
                                      .Not(
                                          n => n
                                              .Modifiers(Modifiers.Private)
                                      )
                                      .ReturnType(ICheckBuilder.GetCurrentEntity)
                             ),
                         a => a
                             .Method(
                                 m => m
                                      .Modifiers(Modifiers.Public)
                                      .Not(
                                          n => n
                                              .Modifiers(Modifiers.Static)
                                      )
                             )
                     )
                     .Not(
                         n => n
                             .Method(
                                 m => m
                                     .Modifiers(Modifiers.Public)
                             )
                     )
            );
    }

    internal IEnumerable< ICheckBuilder > Create()
    {
        yield return Class(
            Any(
                Method()
                    .Modifiers(Modifiers.Static)
                    .Not(Modifiers2(Modifiers.Private))
                    .ReturnType(ICheckBuilder.GetCurrentEntity),
                Method()
                    .Modifiers(Modifiers.Public)
                    .Not(Modifiers2(Modifiers.Static))
            ),
            Not(
                Method()
                    .Modifiers(Modifiers.Public)
            )
        );
    }
}

internal static class CheckBuilder
{
    internal static CheckCollectionBuilder Any(
        params ICheckBuilder[ ] checkBuilders) => new(
        CheckCollectionKind.Any,
        checkBuilders );

    internal static NotCheckBuilder Not(
        ICheckBuilder checkBuilder) => new( checkBuilder );

    internal static ClassCheckBuilder Class(
        params ICheckBuilder[ ] checkBuilders) => new( checkBuilders );

    internal static MethodCheckBuilder Method() => new();

    internal static ModifierCheckBuilder Modifiers2(
        params IModifier[ ] modifiers) => new( modifiers );
}
