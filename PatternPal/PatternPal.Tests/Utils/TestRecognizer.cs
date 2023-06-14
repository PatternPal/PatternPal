#region

using PatternPal.Core.StepByStep;

#endregion

namespace PatternPal.Tests.Utils;

// TODO These are utils, so I'd like to see them commented.
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

    public List<IInstruction> GenerateStepsList()
    {
        throw new NotImplementedException();
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

    public List<IInstruction> GenerateStepsList()
    {
        throw new NotImplementedException();
    }
}
