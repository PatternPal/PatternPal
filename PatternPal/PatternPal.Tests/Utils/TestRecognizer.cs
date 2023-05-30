namespace PatternPal.Tests.Utils;

internal class TestRecognizerRelation : IRecognizer
{
    public string Name => nameof(TestRecognizerRelation);

    public Protos.Recognizer RecognizerType => Protos.Recognizer.Unknown;

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
    public string Name => nameof(TestRecognizerType);

    public Protos.Recognizer RecognizerType => Protos.Recognizer.Unknown;

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
            Method(
                Priority.Knockout,
                Type(
                    Priority.Knockout,
                    internalClass
                )
            )
        );
    }
}
