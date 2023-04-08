using static PatternPal.Core.Recognizers.CheckBuilder;

namespace PatternPal.Core.Recognizers;

internal class SingletonRecognizer
{
    internal IEnumerable< ICheckBuilder > Create()
    {
        MethodCheckBuilder instanceMethod =
            Method()
                .Modifiers(Modifiers.Static);

        yield return Class(
            instanceMethod
        );

        yield return Class(
            Method()
                .Uses(instanceMethod.Result)
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
