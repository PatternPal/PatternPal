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
