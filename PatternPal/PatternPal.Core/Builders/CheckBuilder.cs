namespace PatternPal.Core.Builders;

internal delegate IEntity GetCurrentEntity(
    RecognizerContext ctx);

internal interface ICheckBuilder
{
    internal static readonly GetCurrentEntity GetCurrentEntity = ctx => ctx.CurrentEntity;

    ICheck Build();

    Priority Priority { get; init; }
}

internal abstract class CheckBuilderBase : ICheckBuilder
{
    protected CheckBuilderBase(
        Priority priority)
    {
        Priority = priority;
    }

    public abstract ICheck Build();

    public Priority Priority { get; init; }
}

internal static class CheckBuilder
{
    internal static CheckCollectionBuilder Any(
        Priority priority,
        params ICheckBuilder[ ] checkBuilders) => new(
        priority,
        CheckCollectionKind.Any,
        checkBuilders );

    internal static NotCheckBuilder Not(
        Priority priority,
        ICheckBuilder checkBuilder) => new(
        priority,
        checkBuilder );

    internal static ClassCheckBuilder Class(
        Priority priority,
        params ICheckBuilder[ ] checkBuilders) => new(
        priority,
        checkBuilders );

    internal static ClassCheckBuilder AbstractClass(
        Priority priority,
        params ICheckBuilder[ ] checkBuilders) =>
        new(
            priority,
            checkBuilders.Prepend(
                Modifiers(
                    priority,
                    Modifier.Abstract)) );

    internal static InterfaceCheckBuilder Interface(
        Priority priority,
        params ICheckBuilder[ ] checkBuilders) => new(
        priority,
        checkBuilders );

    internal static MethodCheckBuilder Method(
        Priority priority,
        params ICheckBuilder[ ] checkBuilders) => new(
        priority,
        checkBuilders );

    internal static PropertyCheckBuilder Property(
        Priority priority,
        params ICheckBuilder[ ] checkBuilders) => new(
        priority,
        checkBuilders );

    internal static ModifierCheckBuilder Modifiers(
        Priority priority,
        params IModifier[ ] modifiers) => new(
        priority,
        modifiers );

    internal static ParameterCheckBuilder Parameters(
        Priority priority,
        params TypeCheck[ ] parameterTypes) => new(
        priority,
        parameterTypes );

    internal static TypeCheckBuilder Type(
        Priority priority,
        Func< INode > getNode) => new(
        priority,
        getNode );

    internal static TypeCheckBuilder Type(
        Priority priority,
        GetCurrentEntity getCurrentEntity) => new(
        priority,
        getCurrentEntity );

    internal static UsesCheckBuilder Uses(
        Priority priority,
        Func< INode > getMatchedEntity) => new(
        priority,
        getMatchedEntity );

    internal static FieldCheckBuilder Field(
        Priority priority,
        params ICheckBuilder[ ] checkBuilders) => new(
        priority,
        checkBuilders );

    internal static ConstructorCheckBuilder Constructor(
        Priority priority,
        params ICheckBuilder[ ] checkBuilders) => new(
        priority,
        checkBuilders );
}
