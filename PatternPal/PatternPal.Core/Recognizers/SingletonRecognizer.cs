﻿using static PatternPal.Core.Recognizers.CheckBuilder;

namespace PatternPal.Core.Recognizers;

internal class SingletonRecognizer
{
    internal IEnumerable< ICheckBuilder > Create()
    {
        MethodCheckBuilder instanceMethod =
            Method(
                Modifiers2(Modifiers.Static),
                Not(Modifiers2(Modifiers.Private))
            );

        yield return Class(
            instanceMethod,
            Field(
                Modifiers2(Modifiers.Static, Modifiers.Private)
            )
        );

        yield return Class(
            Method(
                Uses(instanceMethod.Result)
            )
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

    internal static MethodCheckBuilder Method(
        params ICheckBuilder[] checkBuilders) => new (checkBuilders );

    internal static ModifierCheckBuilder Modifiers2(
        params IModifier[ ] modifiers) => new( modifiers );

    internal static UsesCheckBuilder Uses(
        Func<INode> getMatchedEntity) => new(getMatchedEntity);

    internal static FieldCheckBuilder Field(
        params ICheckBuilder[] checkBuilders) => new( checkBuilders );
}

