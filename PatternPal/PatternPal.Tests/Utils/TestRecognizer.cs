namespace PatternPal.Tests.Utils;

internal class TestRecognizer : IRecognizer
{
    public IEnumerable<ICheck> Create()
    {
        MethodCheck instanceMethod =
            Method(
                Priority.Low,
                Modifiers(
                    Priority.Low,
                    Modifier.Static
                ),
                Not(
                    Priority.Low,
                    Modifiers(
                        Priority.Low,
                        Modifier.Private
                    )
                )
            );

        yield return Class(
            Priority.Low,
            instanceMethod,
            Field(
                Priority.Low,
                Modifiers(
                    Priority.Low,
                    Modifier.Static,
                    Modifier.Private
                ),
                Type(
                    Priority.Low,
                    ICheck.GetCurrentEntity
                )
            ),
            Constructor(
                Priority.Low,
                Modifiers(
                    Priority.Low,
                    Modifier.Private
                )
            ),
            Not(
                Priority.Low,
                Any(
                    Priority.Low,
                    Constructor(
                        Priority.Low,
                        Modifiers(
                            Priority.Low,
                            Modifier.Public
                        )
                    ),
                    Constructor(
                        Priority.Low,
                        Modifiers(
                            Priority.Low,
                            Modifier.Internal
                        )
                    ),
                    Constructor(
                        Priority.Low,
                        Modifiers(
                            Priority.Low,
                            Modifier.Protected
                        )
                    )
                )
            )
        );

        yield return Class(
            Priority.Low,
            Method(
                Priority.Low,
                Uses(
                    Priority.Low,
                    instanceMethod)
            )
        );
    }
}
