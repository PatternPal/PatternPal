namespace PatternPal.Core.Checks;

internal delegate IEntity GetCurrentEntity(
    RecognizerContext ctx);

internal interface ICheck
{
    internal static readonly GetCurrentEntity GetCurrentEntity = ctx => ctx.CurrentEntity;

    Priority Priority { get; }

    ICheckResult Check(
        RecognizerContext ctx,
        INode node);
}

internal abstract class CheckBase : ICheck
{
    public Priority Priority { get; init; }

    protected CheckBase(
        Priority priority)
    {
        Priority = priority;
    }

    public abstract ICheckResult Check(
        RecognizerContext ctx,
        INode node);
}

internal enum Priority
{
    Knockout,
    High,
    Mid,
    Low
}

enum CheckCollectionKind
{
    All,
    Any
}

internal static class CheckBuilder
{
    internal static CheckCollection Any(
        Priority priority,
        params ICheck[ ] checks) => new(
        priority,
        CheckCollectionKind.Any,
        checks );

    internal static NotCheck Not(
        Priority priority,
        ICheck check) => new(
        priority,
        check );

    internal static ClassCheck Class(
        Priority priority,
        params ICheck[ ] checks) => new(
        priority,
        checks );

    internal static ClassCheck AbstractClass(
        Priority priority,
        params ICheck[ ] checks) =>
        new(
            priority,
            checks.Prepend(
                Modifiers(
                    priority,
                    Modifier.Abstract)) );

    internal static InterfaceCheck Interface(
        Priority priority,
        params ICheck[ ] checks) => new(
        priority,
        checks );

    internal static MethodCheck Method(
        Priority priority,
        params ICheck[ ] checks) => new(
        priority,
        checks );

    internal static PropertyCheck Property(
        Priority priority,
        params ICheck[ ] checks) => new(
        priority,
        checks );

    internal static ModifierCheck Modifiers(
        Priority priority,
        params IModifier[ ] modifiers) => new(
        priority,
        modifiers );

    internal static ParameterCheck Parameters(
        Priority priority,
        params TypeCheck[ ] parameterTypes) => new(
        priority,
        parameterTypes );

    internal static TypeCheck Type(
        Priority priority,
        Func< List< INode > > getNode) => new(
        priority,
        getNode );

    internal static TypeCheck Type(
        Priority priority,
        GetCurrentEntity getCurrentEntity) => new(
        priority,
        getCurrentEntity );

    internal static UsesCheck Uses(
        Priority priority,
        Func< List< INode > > getMatchedEntities) => new(
        priority,
        getMatchedEntities );

    internal static FieldCheck Field(
        Priority priority,
        params ICheck[ ] checks) => new(
        priority,
        checks );

    internal static ConstructorCheck Constructor(
        Priority priority,
        params ICheck[ ] checks) => new(
        priority,
        checks );
}

/// <summary>
/// Class containing helper methods for implementing <see cref="ICheck"/>s.
/// </summary>
internal static class CheckHelper
{
    /// <summary>
    /// Converts an <see cref="INode"/> to <see cref="T"/> and throws an exception if this is not possible.
    /// </summary>
    /// <param name="node">The <see cref="INode"/> to convert.</param>
    /// <returns>The <see cref="INode"/> instance converted to an instance of type <see cref="T"/>.</returns>
    /// <exception cref="IncorrectNodeTypeException">Thrown if <paramref name="node"/> is not of type <see cref="T"/></exception>
    internal static T ConvertNodeElseThrow< T >(
        INode node)
        where T : INode => node is not T asT
        ? throw IncorrectNodeTypeException.From< T >()
        : asT;

    /// <summary>
    /// Creates a new <see cref="InvalidSubCheckException"/>, which represents an incorrectly nested check.
    /// </summary>
    /// <param name="parentCheck">The parent check.</param>
    /// <param name="subCheck">The sub-check.</param>
    /// <returns>The created <see cref="InvalidSubCheckException"/>.</returns>
    internal static InvalidSubCheckException InvalidSubCheck(
        ICheck parentCheck,
        ICheck subCheck) => new(
        parentCheck,
        subCheck );
}

/// <summary>
/// Represents an error thrown when receiving an incorrect type of <see cref="INode"/> in a check.
/// </summary>
internal sealed class IncorrectNodeTypeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncorrectNodeTypeException"/> class for the given <paramref name="type"/>
    /// </summary>
    /// <param name="type">The type to which the instance could not be converted.</param>
    private IncorrectNodeTypeException(
        Type type)
        : base($"Node must be of type '{type}'")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IncorrectNodeTypeException"/> class for the given <see cref="T"/>.
    /// </summary>
    internal static IncorrectNodeTypeException From< T >() => new( typeof( T ) );
}

/// <summary>
/// Represents an error thrown when a check contains a sub-check which is not supported as a sub-check for that check.
/// </summary>
internal sealed class InvalidSubCheckException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSubCheckException"/> class for the given <paramref name="parentCheck"/> and <paramref name="subCheck"/>.
    /// </summary>
    /// <param name="parentCheck">The parent check.</param>
    /// <param name="subCheck">The sub-check.</param>
    internal InvalidSubCheckException(
        ICheck parentCheck,
        ICheck subCheck)
        : base($"'{parentCheck}' cannot contain a '{subCheck}' check")
    {
    }
}
