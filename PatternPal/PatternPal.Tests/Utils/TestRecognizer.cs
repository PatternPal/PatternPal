namespace PatternPal.Tests.Utils;

internal class TestRecognizer : IRecognizer
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
