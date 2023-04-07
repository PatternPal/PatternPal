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
                                      .Modifiers(
                                          Modifiers.Static
                                      )
                                      .Not(
                                          n => n
                                              .Modifiers(Modifiers.Private)
                                      )
                                      .ReturnType(ICheckBuilder.GetCurrentEntity)
                             ),
                         a => a
                             .Method(
                                 m => m
                                      .Modifiers(
                                          Modifiers.Public
                                      )
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
}
