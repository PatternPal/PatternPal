namespace PatternPal.Tests.Utils;

internal class TestRecognizerRelation : IRecognizer
{
    public IEnumerable<ICheck> Create()
    {
        MethodCheck instanceMethod =
            Method(
                Priority.Knockout,
                Modifiers(
                    Priority.Knockout,
                    Modifier.Static
                )
            );

        yield return Class(
            Priority.Knockout,
            instanceMethod
        );

        yield return Class(
            Priority.Knockout,
            Uses(
                Priority.Knockout,
                instanceMethod
            )
        );
    }
}

internal class TestRecognizerType : IRecognizer
{
    public IEnumerable<ICheck> Create()
    {
        ClassCheck internalClass = Class(
            Priority.Knockout,
            Modifiers(
                Priority.Knockout,
                Modifier.Internal
            )
        );

        yield return internalClass;

        yield return Class(
            Priority.Knockout,
            Uses(
                Priority.Knockout,
                Method(
                    Priority.Knockout,
                    Type(
                        Priority.Knockout,
                        internalClass
                    )
                )
            )
        );
    }
}
