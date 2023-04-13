namespace PatternPal.Core.Builders;

internal delegate IEntity GetCurrentEntity(
    RecognizerContext ctx);

internal interface ICheckBuilder
{
    internal static readonly GetCurrentEntity GetCurrentEntity = ctx => ctx.CurrentEntity;

    ICheck Build();
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

    internal static ClassCheckBuilder AbstractClass(
        params ICheckBuilder[] checkBuilders) => new(checkBuilders.Prepend(Modifiers(Modifier.Abstract)));

    internal static InterfaceCheckBuilder Interface(
        params ICheckBuilder[] checkBuilders) => new(checkBuilders);

    internal static MethodCheckBuilder Method(
        params ICheckBuilder[ ] checkBuilders) => new( checkBuilders );

    internal static PropertyCheckBuilder Property(
        params ICheckBuilder[ ] checkBuilders) => new( checkBuilders );

    internal static ModifierCheckBuilder Modifiers(
        params IModifier[ ] modifiers) => new( modifiers );

    internal static ParameterCheckBuilder Parameters(
        params TypeCheck[ ] parameterTypes) => new( parameterTypes );

    internal static UsesCheckBuilder Uses(
        Func< INode > getMatchedEntity) => new( getMatchedEntity );

    internal static FieldCheckBuilder Field(
        params ICheckBuilder[ ] checkBuilders) => new( checkBuilders );

    internal static ConstructorCheckBuilder Constructor(
        params ICheckBuilder[ ] checkBuilders) => new( checkBuilders );
}
