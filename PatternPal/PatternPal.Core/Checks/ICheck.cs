namespace PatternPal.Core.Checks;

/// <summary>
/// Function which can be called to get an <see cref="IEntity"/>.
/// </summary>
internal delegate IEntity GetCurrentEntity(
    RecognizerContext ctx);

/// <summary>
/// Represents a check, which is a part of a <see cref="Recognizers.IRecognizer"/> responsible for checking
/// something in the <see cref="SyntaxGraph"/>, e.g. the modifiers of a method, or the type of a
/// property.
/// </summary>
internal interface ICheck
{
    /// <summary>
    /// Function which will return the current <see cref="IEntity"/> being checked.
    /// </summary>
    /// <param name="ctx">The current <see cref="RecognizerContext"/>.</param>
    /// <returns>The current <see cref="IEntity"/> being checked.</returns>
    internal static readonly GetCurrentEntity GetCurrentEntity = ctx => ctx.CurrentEntity;

    /// <summary>
    /// The <see cref="Checks.Priority"/> of this check.
    /// </summary>
    Priority Priority { get; }

    /// <summary>
    /// Runs the current check on the given <see cref="INode"/>.
    /// </summary>
    /// <param name="ctx">The current <see cref="RecognizerContext"/>.</param>
    /// <param name="node">The <see cref="INode"/> to be checked.</param>
    /// <returns>An <see cref="ICheckResult"/> which represents the result of the check.</returns>
    ICheckResult Check(
        RecognizerContext ctx,
        INode node);
}

/// <summary>
/// Base implementation of a check.
/// </summary>
internal abstract class CheckBase : ICheck
{
    /// <inheritdoc />
    public Priority Priority { get; }

    /// <summary>
    /// Sets the priority.
    /// </summary>
    protected CheckBase(
        Priority priority)
    {
        Priority = priority;
    }

    /// <inheritdoc />
    public abstract ICheckResult Check(
        RecognizerContext ctx,
        INode node);
}

/// <summary>
/// Represents the priority of a <see cref="ICheck"/>.
/// </summary>
public enum Priority
{
    /// <summary>
    /// This <see cref="ICheck"/> is required for the <see cref="Recognizers.IRecognizer"/> to succeed.
    /// </summary>
    Knockout,

    /// <summary>
    /// This <see cref="ICheck"/> is very important.
    /// </summary>
    High,

    /// <summary>
    /// This <see cref="ICheck"/> is moderately important.
    /// </summary>
    Mid,

    /// <summary>
    /// This <see cref="ICheck"/> is optional.
    /// </summary>
    Low
}

/// <summary>
/// Represents the behavior of a collection of <see cref="ICheck"/>s when deciding if it is correct.
/// </summary>
public enum CheckCollectionKind
{
    /// <summary>
    /// All sub-<see cref="ICheck"/>s must succeed for this <see cref="ICheck"/> to succeed.
    /// </summary>
    All,

    /// <summary>
    /// At least one of the sub-<see cref="ICheck"/>s must succeed for this <see cref="ICheck"/> to succeed.
    /// </summary>
    Any
}

/// <summary>
/// Helper class for creating checks.
/// </summary>
internal static class CheckBuilder
{
    /// <summary>
    /// Creates a new <see cref="CheckCollection"/> of kind <see cref="CheckCollectionKind.Any"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="CheckCollection"/>.</param>
    /// <param name="checks">The sub-<see cref="ICheck"/>s of this <see cref="CheckCollection"/>.</param>
    /// <returns>The created <see cref="CheckCollection"/>.</returns>
    internal static CheckCollection Any(
        Priority priority,
        params ICheck[ ] checks) => new(
        priority,
        CheckCollectionKind.Any,
        checks );

    /// <summary>
    /// Creates a new <see cref="CheckCollection"/> of kind <see cref="CheckCollectionKind.All"/>
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="CheckCollection"/>.</param>
    /// <param name="checks">The sub-<see cref="ICheck"/>s of this <see cref="CheckCollection"/>.</param>
    /// <returns>The created <see cref="CheckCollection"/>.</returns>
    internal static CheckCollection All(
        Priority priority,
        params ICheck[] checks) => new(priority, CheckCollectionKind.All, checks);

    /// <summary>
    /// Creates a new <see cref="NotCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="NotCheck"/>.</param>
    /// <param name="check">The <see cref="ICheck"/> to be inverted by the <see cref="NotCheck"/>.</param>
    /// <returns>The created <see cref="NotCheck"/>.</returns>
    internal static NotCheck Not(
        Priority priority,
        ICheck check) => new(
        priority,
        check );

    /// <summary>
    /// Creates a new <see cref="ClassCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="ClassCheck"/>.</param>
    /// <param name="checks"/>The sub-<see cref="ICheck"/>s of this <see cref="ClassCheck"/>.</param>
    /// <returns>The created <see cref="ClassCheck"/>.</returns>
    internal static ClassCheck Class(
        Priority priority,
        params ICheck[ ] checks) => new(
        priority,
        checks );

    /// <summary>
    /// Creates a new <see cref="ClassCheck"/> with a <see cref="Modifier"/> check for <see langword="abstract"/> as it first sub-<see cref="ICheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="ClassCheck"/>.</param>
    /// <param name="checks"/>The sub-<see cref="ICheck"/>s of this <see cref="ClassCheck"/>.</param>
    /// <returns>The created <see cref="ClassCheck"/>.</returns>
    internal static ClassCheck AbstractClass(
        Priority priority,
        params ICheck[ ] checks) =>
        new(
            priority,
            checks.Prepend(
                Modifiers(
                    priority,
                    Modifier.Abstract)) );

    /// <summary>
    /// Creates a new <see cref="InterfaceCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="InterfaceCheck"/>.</param>
    /// <param name="checks"/>The sub-<see cref="ICheck"/>s of this <see cref="InterfaceCheck"/>.</param>
    /// <returns>The created <see cref="InterfaceCheck"/>.</returns>
    internal static InterfaceCheck Interface(
        Priority priority,
        params ICheck[ ] checks) => new(
        priority,
        checks );

    /// <summary>
    /// Creates a new <see cref="MethodCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="MethodCheck"/>.</param>
    /// <param name="checks"/>The sub-<see cref="ICheck"/>s of this <see cref="MethodCheck"/>.</param>
    /// <returns>The created <see cref="MethodCheck"/>.</returns>
    internal static MethodCheck Method(
        Priority priority,
        params ICheck[ ] checks) => new(
        priority,
        checks );

    /// <summary>
    /// Creates a new <see cref="PropertyCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="PropertyCheck"/>.</param>
    /// <param name="checks"/>The sub-<see cref="ICheck"/>s of this <see cref="PropertyCheck"/>.</param>
    /// <returns>The created <see cref="PropertyCheck"/>.</returns>
    internal static PropertyCheck Property(
        Priority priority,
        params ICheck[ ] checks) => new(
        priority,
        checks );

    /// <summary>
    /// Creates a new <see cref="ModifierCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="ModifierCheck"/>.</param>
    /// <param name="modifiers">The <see cref="Modifier"/> which this <see cref="ModifierCheck"/> should check for.</param>
    /// <returns>The created <see cref="ModifierCheck"/>.</returns>
    internal static ModifierCheck Modifiers(
        Priority priority,
        params IModifier[ ] modifiers) => new(
        priority,
        modifiers );

    /// <summary>
    /// Creates a new <see cref="ParameterCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="ParameterCheck"/>.</param>
    /// <param name="parameterTypes"><see cref="TypeCheck"/>s which are used to check the parameters.</param>
    /// <returns>The created <see cref="ParameterCheck"/>.</returns>
    internal static ParameterCheck Parameters(
        Priority priority,
        params TypeCheck[ ] parameterTypes) => new(
        priority,
        parameterTypes );

    /// <summary>
    /// Creates a new <see cref="TypeCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="TypeCheck"/>.</param>
    /// <param name="getMatchedNodes">A <see cref="Func{TResult}"/> which returns a <see cref="List{T}"/> of matched <see cref="INode"/>s from another <see cref="ICheck"/>.</param>
    /// <returns>The created <see cref="TypeCheck"/>.</returns>
    internal static TypeCheck Type(
        Priority priority,
        Func< List< INode > > getMatchedNodes) => new(
        priority,
        getMatchedNodes );

    /// <summary>
    /// Creates a new <see cref="TypeCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="TypeCheck"/>.</param>
    /// <param name="getCurrentEntity">A <see cref="GetCurrentEntity"/> which returns the current <see cref="IEntity"/> being checked.</param>
    /// <returns>The created <see cref="TypeCheck"/>.</returns>
    internal static TypeCheck Type(
        Priority priority,
        GetCurrentEntity getCurrentEntity) => new(
        priority,
        getCurrentEntity );

    /// <summary>
    /// Creates a new <see cref="RelationCheck"/> for a uses relation.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="RelationCheck"/>.</param>
    /// <param name="getMatchedNodes">A <see cref="Func{TResult}"/> which returns a <see cref="List{T}"/> of matched <see cref="INode"/>s from another <see cref="ICheck"/>.</param>
    /// <returns>The created <see cref="RelationCheck"/>.</returns>
    internal static RelationCheck Uses(
        Priority priority,
        Func< List< INode > > getMatchedNodes) => new(
        priority,
        RelationType.Uses,
        getMatchedNodes );

    /// <summary>
    /// Creates a new <see cref="RelationCheck"/> for a inheritance relation.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="RelationCheck"/>.</param>
    /// <param name="getMatchedNodes">A <see cref="Func{TResult}"/> which returns a <see cref="List{T}"/> of matched <see cref="INode"/>s from another <see cref="ICheck"/>.</param>
    /// <returns>The created <see cref="RelationCheck"/>.</returns>
    internal static RelationCheck Inherits(
        Priority priority,
        Func<List<INode>> getMatchedNodes) => new(
        priority,
        RelationType.Extends,
        getMatchedNodes);

    /// <summary>
    /// Creates a new <see cref="RelationCheck"/> for an implements relation.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="RelationCheck"/>.</param>
    /// <param name="getMatchedNodes">A <see cref="Func{TResult}"/> which returns a <see cref="List{T}"/> of matched <see cref="INode"/>s from another <see cref="ICheck"/>.</param>
    /// <returns>The created <see cref="RelationCheck"/>.</returns>
    internal static RelationCheck Implements(
        Priority priority,
        Func<List<INode>> getMatchedNodes) => new(
        priority,
        RelationType.Implements,
        getMatchedNodes);

    /// <summary>
    /// Creates a new <see cref="RelationCheck"/> for a creation relation.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="RelationCheck"/>.</param>
    /// <param name="getMatchedNodes">A <see cref="Func{TResult}"/> which returns a <see cref="List{T}"/> of matched <see cref="INode"/>s from another <see cref="ICheck"/>.</param>
    /// <returns>The created <see cref="RelationCheck"/>.</returns>
    internal static RelationCheck Creates(
        Priority priority,
        Func<List<INode>> getMatchedNodes) => new(
        priority,
        RelationType.Creates,
        getMatchedNodes);

    /// <summary>
    /// Creates a new <see cref="FieldCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="FieldCheck"/>.</param>
    /// <param name="checks"/>The sub-<see cref="ICheck"/>s of this <see cref="FieldCheck"/>.</param>
    /// <returns>The created <see cref="FieldCheck"/>.</returns>
    internal static FieldCheck Field(
        Priority priority,
        params ICheck[ ] checks) => new(
        priority,
        checks );

    /// <summary>
    /// Creates a new <see cref="ConstructorCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="ConstructorCheck"/>.</param>
    /// <param name="checks"/>The sub-<see cref="ICheck"/>s of this <see cref="ConstructorCheck"/>.</param>
    /// <returns>The created <see cref="ConstructorCheck"/>.</returns>
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

    /// <summary>
    /// This method checks the value of the leaf nodes and returns true if all the children are correct and false in other cases
    /// </summary>
    internal static bool CheckAllChildrenCorrect(ICheckResult checkResult)
    {
        if (checkResult is LeafCheckResult leafResult)
        {
            return leafResult.Correct;
        }

        if (checkResult is NodeCheckResult nodeResult)
        {
            return nodeResult.ChildrenCheckResults.All(childResult => CheckAllChildrenCorrect(childResult));
        }

        throw new ArgumentException($"Unknown ICheckResult type: {checkResult.GetType().Name}");

    }
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
