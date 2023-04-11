using static PatternPal.Core.Builders.CheckBuilder;

namespace PatternPal.Core.Recognizers;

internal class SingletonRecognizer
{
    internal IEnumerable< ICheckBuilder > Create()
    {
        MethodCheckBuilder instanceMethod =
            Method(
                Modifiers(Modifier.Static),
                Not(Modifiers(Modifier.Private))
            );

        yield return Class(
            instanceMethod,
            Field(
                Modifiers(
                    Modifier.Static,
                    Modifier.Private)
            ),
            Constructor(
                Modifiers(Modifier.Private)
            ),
            Not(
                Any(
                    Constructor(
                        Modifiers(Modifier.Public)
                    ),
                    Constructor(
                        Modifiers(Modifier.Internal)
                    ),
                    Constructor(
                        Modifiers(Modifier.Protected)
                    )
                )
            )
        );

        yield return Class(
            Method(
                Uses(instanceMethod.Result)
            )
        );
    }
}
