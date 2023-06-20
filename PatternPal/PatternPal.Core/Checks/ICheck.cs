#region

using PatternPal.SyntaxTree;
using PatternPal.SyntaxTree.Models;

#endregion

namespace PatternPal.Core.Checks;

/// <summary>
/// Function which can be called to get an <see cref="IEntity"/>.
/// </summary>
internal delegate IEntity GetCurrentEntity(
    IRecognizerContext ctx);

/// <summary>
/// Represents a check, which is a part of a <see cref="Recognizers.IRecognizer"/> responsible for checking
/// some properties in the <see cref="SyntaxGraph"/>, e.g. the modifiers of a method, or the type of a
/// property.
/// </summary>
public interface ICheck
{
    /// <summary>
    /// Function which will return the current <see cref="IEntity"/> being checked.
    /// </summary>
    /// <param name="ctx">The current <see cref="IRecognizerContext"/>.</param>
    /// <returns>The current <see cref="IEntity"/> being checked.</returns>
    internal static readonly GetCurrentEntity GetCurrentEntity = ctx => ctx.CurrentEntity;

    /// <summary>
    /// The <see cref="Checks.Priority"/> of this check.
    /// </summary>
    Priority Priority { get; }

    /// <summary>
    /// The requirement to which this <see cref="ICheck"/> belongs.
    /// </summary>
    string ? Requirement { get; }

    /// <summary>
    /// Returns a <see cref="Func{TResult}"/> which can be invoked to get the results of this <see cref="ICheck"/>.
    /// </summary>
    Func< List< INode > > Result { get; }

    /// <summary>
    /// The dependencies to other <see cref="INode"/>s this check has.
    /// </summary>
    int DependencyCount { get; }

    /// <summary>
    /// The <see cref="ICheck"/> in which this <see cref="ICheck"/> is contained.
    /// </summary>
    ICheck ? ParentCheck { get; set; }

    /// <summary>
    /// Calculates the Perfect Score of this <see cref="ICheck"/>. The <paramref name="result"/> is
    /// used to decide which sub-<see cref="ICheck"/> to include when the <see cref="CheckCollectionKind"/>
    /// is <see cref="CheckCollectionKind.Any"/>.
    /// </summary>
    /// <param name="resultsByCheck">Contains the <see cref="ICheckResult"/> for each <see cref="ICheck"/>.</param>
    /// <param name="result">The <see cref="ICheckResult"/> of this <see cref="ICheck"/>.</param>
    /// <returns>
    /// The <see cref="Score"/> of the <see cref="ICheckResult"/> of this <see cref="ICheck"/> when
    /// it is implemented correctly.
    /// </returns>
    Score PerfectScore { get; }

    /// <summary>
    /// Runs the current check on the given <see cref="INode"/>.
    /// </summary>
    /// <param name="ctx">The current <see cref="IRecognizerContext"/>.</param>
    /// <param name="node">The <see cref="INode"/> to be checked.</param>
    /// <returns>An <see cref="ICheckResult"/> which represents the result of the check.</returns>
    ICheckResult Check(
        IRecognizerContext ctx,
        INode node);
}

/// <summary>
/// Base implementation of a check.
/// </summary>
public abstract class CheckBase : ICheck
{
    /// <inheritdoc />
    public Priority Priority { get; }

    /// <inheritdoc />
    public string ? Requirement { get; }

    /// <inheritdoc />
    public virtual Func< List< INode > > Result => throw new NotSupportedException($"this check '{this}' is not a NodeCheck");

    /// <inheritdoc />
    public abstract int DependencyCount { get; }

    /// <inheritdoc />
    public ICheck ? ParentCheck { get; set; }

    /// <inheritdoc />
    public abstract Score PerfectScore { get; }

    /// <summary>
    /// Sets the priority.
    /// </summary>
    /// <param name="priority">The <see cref="Checks.Priority"/> of this <see cref="ICheck"/>.</param>
    /// <param name="requirement">The optional requirement which this <see cref="ICheck"/> checks.</param>
    protected CheckBase(
        Priority priority,
        string ? requirement)
    {
        Priority = priority;
        Requirement = requirement;
    }

    /// <inheritdoc />
    public abstract ICheckResult Check(
        IRecognizerContext ctx,
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
    /// Creates a new <see cref="NodeCheck{TNode}"/> of kind <see cref="CheckCollectionKind.Any"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="NodeCheck{TNode}"/>.</param>
    /// <param name="checks">The sub-<see cref="ICheck"/>s of this <see cref="NodeCheck{TNode}"/>.</param>
    /// <returns>The created <see cref="NodeCheck{TNode}"/>.</returns>
    internal static NodeCheck< INode > Any(
        Priority priority,
        params ICheck[ ] checks) => Any(
        priority,
        null,
        checks);

    /// <inheritdoc cref="Any"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static NodeCheck< INode > Any(
        Priority priority,
        string ? requirement,
        params ICheck[ ] checks) => new(
        priority,
        requirement,
        checks,
        CheckCollectionKind.Any );

    /// <summary>
    /// Creates a new <see cref="NodeCheck{TNode}"/> of kind <see cref="CheckCollectionKind.All"/>
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="NodeCheck{TNode}"/>.</param>
    /// <param name="checks">The sub-<see cref="ICheck"/>s of this <see cref="NodeCheck{TNode}"/>.</param>
    /// <returns>The created <see cref="NodeCheck{TNode}"/>.</returns>
    internal static NodeCheck< INode > All(
        Priority priority,
        params ICheck[ ] checks) => All(
        priority,
        null,
        checks);

    /// <inheritdoc cref="All"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static NodeCheck< INode > All(
        Priority priority,
        string ? requirement,
        params ICheck[ ] checks) => new(
        priority,
        requirement,
        checks );

    /// <summary>
    /// Creates a new <see cref="NotCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="NotCheck"/>.</param>
    /// <param name="hasNoPublicInternalConstructor"></param>
    /// <param name="check">The <see cref="ICheck"/> to be inverted by the <see cref="NotCheck"/>.</param>
    /// <returns>The created <see cref="NotCheck"/>.</returns>
    internal static NotCheck Not(
        Priority priority,
        ICheck check) => Not(
        priority,
        null,
        check);

    /// <inheritdoc cref="Not"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static NotCheck Not(
        Priority priority,
        string ? requirement,
        ICheck check) => new(
        priority,
        requirement,
        check );

    /// <summary>
    /// Creates a new <see cref="ClassCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="ClassCheck"/>.</param>
    /// <param name="checks">The sub-<see cref="ICheck"/>s of this <see cref="ClassCheck"/>.</param>
    /// <returns>The created <see cref="ClassCheck"/>.</returns>
    internal static ClassCheck Class(
        Priority priority,
        params ICheck[ ] checks) => Class(
        priority,
        null,
        checks);

    /// <inheritdoc cref="Class"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static ClassCheck Class(
        Priority priority,
        string ? requirement,
        params ICheck[ ] checks) => new(
        priority,
        requirement,
        checks );

    /// <summary>
    /// Creates a new <see cref="ClassCheck"/> with a <see cref="Modifier"/> check for <see langword="abstract"/> as it first sub-<see cref="ICheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="ClassCheck"/>.</param>
    /// <param name="checks">The sub-<see cref="ICheck"/>s of this <see cref="ClassCheck"/>.</param>
    /// <returns>The created <see cref="ClassCheck"/>.</returns>
    internal static ClassCheck AbstractClass(
        Priority priority,
        params ICheck[ ] checks) =>
        AbstractClass(
            priority,
            null,
            checks);

    /// <inheritdoc cref="AbstractClass"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static ClassCheck AbstractClass(
        Priority priority,
        string ? requirement,
        params ICheck[ ] checks) =>
        new(
            priority,
            requirement,
            checks.Prepend(
                Modifiers(
                    priority,
                    Modifier.Abstract)) );

    /// <summary>
    /// Creates a new <see cref="InterfaceCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="InterfaceCheck"/>.</param>
    /// <param name="checks">The sub-<see cref="ICheck"/>s of this <see cref="InterfaceCheck"/>.</param>
    /// <returns>The created <see cref="InterfaceCheck"/>.</returns>
    internal static InterfaceCheck Interface(
        Priority priority,
        params ICheck[ ] checks) => Interface(
        priority,
        null,
        checks);

    /// <inheritdoc cref="Interface"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static InterfaceCheck Interface(
        Priority priority,
        string ? requirement,
        params ICheck[ ] checks) => new(
        priority,
        requirement,
        checks );

    /// <summary>
    /// Creates a new <see cref="MethodCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="MethodCheck"/>.</param>
    /// <param name="checks">The sub-<see cref="ICheck"/>s of this <see cref="MethodCheck"/>.</param>
    /// <returns>The created <see cref="MethodCheck"/>.</returns>
    internal static MethodCheck Method(
        Priority priority,
        params ICheck[ ] checks) => Method(
        priority,
        null,
        checks);

    /// <inheritdoc cref="Method"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static MethodCheck Method(
        Priority priority,
        string ? requirement,
        params ICheck[ ] checks) => new(
        priority,
        requirement,
        checks );

    /// <summary>
    /// Creates a new <see cref="PropertyCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="PropertyCheck"/>.</param>
    /// <param name="checks">The sub-<see cref="ICheck"/>s of this <see cref="PropertyCheck"/>.</param>
    /// <returns>The created <see cref="PropertyCheck"/>.</returns>
    internal static PropertyCheck Property(
        Priority priority,
        params ICheck[ ] checks) => Property(
        priority,
        null,
        checks);

    /// <inheritdoc cref="Property"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static PropertyCheck Property(
        Priority priority,
        string ? requirement,
        params ICheck[ ] checks) => new(
        priority,
        requirement,
        checks );

    /// <summary>
    /// Creates a new <see cref="ModifierCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="ModifierCheck"/>.</param>
    /// <param name="modifiers">The <see cref="Modifier"/> which this <see cref="ModifierCheck"/> should check for.</param>
    /// <returns>The created <see cref="ModifierCheck"/>.</returns>
    internal static ModifierCheck Modifiers(
        Priority priority,
        params IModifier[ ] modifiers) => Modifiers(
        priority,
        null,
        modifiers);

    /// <inheritdoc cref="Modifiers"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static ModifierCheck Modifiers(
        Priority priority,
        string ? requirement,
        params IModifier[ ] modifiers) => new(
        priority,
        requirement,
        modifiers );

    /// <summary>
    /// Creates a new <see cref="ParameterCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="ParameterCheck"/>.</param>
    /// <param name="parameterTypes"><see cref="TypeCheck"/>s which are used to check the parameters.</param>
    /// <returns>The created <see cref="ParameterCheck"/>.</returns>
    internal static ParameterCheck Parameters(
        Priority priority,
        params TypeCheck[ ] parameterTypes) => Parameters(
        priority,
        null,
        parameterTypes);

    /// <inheritdoc cref="Parameters"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static ParameterCheck Parameters(
        Priority priority,
        string ? requirement,
        params TypeCheck[ ] parameterTypes) => new(
        priority,
        requirement,
        parameterTypes );

    /// <summary>
    /// Creates a new <see cref="TypeCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="TypeCheck"/>.</param>
    /// <param name="getRelatedCheck">A <see cref="CheckBase"/> which belongs to the <see cref="ICheck"/> which is used as type reference.</param>
    /// <returns>The created <see cref="TypeCheck"/>.</returns>
    internal static TypeCheck Type(
        Priority priority,
        CheckBase getRelatedCheck) => Type(
        priority,
        null,
        getRelatedCheck);

    /// <inheritdoc cref="Type"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static TypeCheck Type(
        Priority priority,
        string ? requirement,
        CheckBase getRelatedCheck) => new(
        priority,
        requirement,
        getRelatedCheck );

    /// <summary>
    /// Creates a new <see cref="TypeCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="TypeCheck"/>.</param>
    /// <param name="getCurrentEntity">A <see cref="GetCurrentEntity"/> which returns the current <see cref="IEntity"/> being checked.</param>
    /// <returns>The created <see cref="TypeCheck"/>.</returns>
    internal static TypeCheck Type(
        Priority priority,
        GetCurrentEntity getCurrentEntity) => Type(
        priority,
        null,
        getCurrentEntity);

    /// <inheritdoc cref="Type"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static TypeCheck Type(
        Priority priority,
        string ? requirement,
        GetCurrentEntity getCurrentEntity) => new(
        priority,
        requirement,
        getCurrentEntity );

    /// <summary>
    /// Creates a new <see cref="RelationCheck"/> for a <see cref="RelationType.Uses"/> relation.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="RelationCheck"/>.</param>
    /// <param name="relatedNodeCheck">The <see cref="ICheck"/> which checks for the node
    /// to which there should be a <see cref="RelationType.Uses"/> relation.</param>
    /// <returns>The created <see cref="RelationCheck"/>.</returns>
    internal static RelationCheck Uses(
        Priority priority,
        ICheck relatedNodeCheck) => Relation(
        priority,
        null,
        RelationType.Uses,
        relatedNodeCheck);

    /// <inheritdoc cref="Uses"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static RelationCheck Uses(
        Priority priority,
        string ? requirement,
        ICheck relatedNodeCheck) => Relation(
        priority,
        requirement,
        RelationType.Uses,
        relatedNodeCheck);

    /// <inheritdoc cref="Relation"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    /// <param name="relationType">The <see cref="RelationType"/> of the relation.</param>
    private static RelationCheck Relation(
        Priority priority,
        string ? requirement,
        RelationType relationType,
        ICheck relatedNodeCheck) => new(
        priority,
        requirement,
        relationType,
        relatedNodeCheck );

    /// <summary>
    /// Creates a new <see cref="RelationCheck"/> for a <see cref="RelationType.UsedBy"/> relation.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="RelationCheck"/>.</param>
    /// <param name="relatedNodeCheck">A <see cref="ICheck"/> which checks for the node from where there should
    /// be a used relation.</param>
    /// <returns></returns>
    internal static RelationCheck UsedBy(
        Priority priority,
        ICheck relatedNodeCheck) => UsedBy(
        priority,
        null,
        relatedNodeCheck);

    /// <inheritdoc cref="UsedBy"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static RelationCheck UsedBy(
        Priority priority,
        string ? requirement,
        ICheck relatedNodeCheck) => Relation(
        priority,
        requirement,
        RelationType.UsedBy,
        relatedNodeCheck);

    /// <summary>
    /// Creates a new <see cref="RelationCheck"/> for a <see cref="RelationType.Extends"/> relation.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="RelationCheck"/>.</param>
    /// <param name="relatedNodeCheck">The <see cref="ICheck"/> which checks for the node
    /// to which there should be a <see cref="RelationType.Extends"/> relation.</param>
    /// <returns>The created <see cref="RelationCheck"/>.</returns>
    internal static RelationCheck Inherits(
        Priority priority,
        ICheck relatedNodeCheck) => Inherits(
        priority,
        null,
        relatedNodeCheck);

    /// <inheritdoc cref="Inherits"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static RelationCheck Inherits(
        Priority priority,
        string ? requirement,
        ICheck relatedNodeCheck) => Relation(
        priority,
        requirement,
        RelationType.Extends,
        relatedNodeCheck);

    /// <summary>
    /// Creates a new <see cref="RelationCheck"/> for a <see cref="RelationType.Implements"/> relation.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="RelationCheck"/>.</param>
    /// <param name="relatedNodeCheck">The <see cref="ICheck"/> which checks for the node
    /// to which there should be a <see cref="RelationType.Implements"/> relation.</param>
    /// <returns>The created <see cref="RelationCheck"/>.</returns>
    internal static RelationCheck Implements(
        Priority priority,
        ICheck relatedNodeCheck) => Implements(
        priority,
        null,
        relatedNodeCheck);

    /// <inheritdoc cref="Implements"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static RelationCheck Implements(
        Priority priority,
        string ? requirement,
        ICheck relatedNodeCheck) => Relation(
        priority,
        requirement,
        RelationType.Implements,
        relatedNodeCheck);

    /// <summary>
    /// Creates a new <see cref="RelationCheck"/> for a <see cref="RelationType.Overrides"/> relation.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="RelationCheck"/>.</param>
    /// <param name="relatedNodeCheck">The <see cref="ICheck"/> which checks for the node
    /// to which there should be a <see cref="RelationType.Overrides"/> relation.</param>
    /// <returns>The created <see cref="RelationCheck"/>.</returns>
    internal static RelationCheck Overrides(
        Priority priority,
        ICheck relatedNodeCheck) => Overrides(
        priority,
        null,
        relatedNodeCheck);

    /// <inheritdoc cref="Overrides"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static RelationCheck Overrides(
        Priority priority,
        string ? requirement,
        ICheck relatedNodeCheck) => Relation(
        priority,
        requirement,
        RelationType.Overrides,
        relatedNodeCheck);

    /// <summary>
    /// Creates a new <see cref="RelationCheck"/> for a <see cref="RelationType.Creates"/> relation.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="RelationCheck"/>.</param>
    /// <param name="relatedNodeCheck">The <see cref="ICheck"/> which checks for the node
    /// to which there should be a <see cref="RelationType.Creates"/> relation.</param>
    /// <returns>The created <see cref="RelationCheck"/></returns>
    internal static RelationCheck Creates(
        Priority priority,
        ICheck relatedNodeCheck) => Creates(
        priority,
        null,
        relatedNodeCheck);

    /// <inheritdoc cref="Creates"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static RelationCheck Creates(
        Priority priority,
        string ? requirement,
        ICheck relatedNodeCheck) => Relation(
        priority,
        requirement,
        RelationType.Creates,
        relatedNodeCheck);

    /// <summary>
    /// Creates a new <see cref="FieldCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="FieldCheck"/>.</param>
    /// <param name="checks">The sub-<see cref="ICheck"/>s of this <see cref="FieldCheck"/>.</param>
    /// <returns>The created <see cref="FieldCheck"/>.</returns>
    internal static FieldCheck Field(
        Priority priority,
        params ICheck[ ] checks) => Field(
        priority,
        null,
        checks);

    /// <inheritdoc cref="Field"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static FieldCheck Field(
        Priority priority,
        string ? requirement,
        params ICheck[ ] checks) => new(
        priority,
        requirement,
        checks );

    /// <summary>
    /// Creates a new <see cref="ConstructorCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of this <see cref="ConstructorCheck"/>.</param>
    /// <param name="checks">The sub-<see cref="ICheck"/>s of this <see cref="ConstructorCheck"/>.</param>
    /// <returns>The created <see cref="ConstructorCheck"/>.</returns>
    internal static ConstructorCheck Constructor(
        Priority priority,
        params ICheck[ ] checks) => Constructor(
        priority,
        null,
        checks);

    /// <inheritdoc cref="Constructor"/>
    /// <param name="requirement">The requirement which this <see cref="ICheck"/> checks for.</param>
    internal static ConstructorCheck Constructor(
        Priority priority,
        string ? requirement,
        params ICheck[ ] checks) => new(
        priority,
        requirement,
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
        ? throw IncorrectNodeTypeException.From< T >(node)
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
    /// <param name="node">The <see cref="INode"/> which could not be converted.</param>
    private IncorrectNodeTypeException(
        Type type,
        INode ? node)
        : base($"Node must be of type '{type}', but is of type '{(node is null ? "<null>" : node.GetType())}'")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IncorrectNodeTypeException"/> class for the given <see cref="T"/>.
    /// </summary>
    /// <param name="node">The <see cref="INode"/> which could not be converted.</param>
    internal static IncorrectNodeTypeException From< T >(
        INode ? node) => new(
        typeof( T ),
        node );
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
