namespace PatternPal.Tests.Utils;


/// <summary>
/// A recognizer that can be used to test the <see cref="RelationCheck"/> of <see cref="RelationType.Uses"/>
/// between a class and a method
/// </summary>
internal class TestRecognizerRelation : IRecognizer
{
    /// <inheritdoc />
    public string Name => nameof(TestRecognizerRelation);

    /// <inheritdoc />
    public Protos.Recognizer RecognizerType => Protos.Recognizer.Unknown;

    /// <inheritdoc />
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

/// <summary>
/// A recognizer that can be used to test the <see cref="TypeCheck"/> in combination with a <see cref="MethodCheck"/>
/// </summary>
internal class TestRecognizerType : IRecognizer
{
    /// <inheritdoc />
    public string Name => nameof(TestRecognizerType);

    /// <inheritdoc />
    public Protos.Recognizer RecognizerType => Protos.Recognizer.Unknown;

    /// <inheritdoc />
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
