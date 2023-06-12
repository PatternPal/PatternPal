#region

using System.ComponentModel;
using PatternPal.Core.StepByStep;
using PatternPal.SyntaxTree.Models;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided files or project implements the decorator pattern
/// </summary>
/// <remarks>
/// 1) Requirements for Component:
///     a) is an interface / abstract class
///     b) has declared a method
///         i) if the class is an abstract class instead of an interface the method has to be an abstract method
/// 2) Requirements for Concrete Component:
///     a) is an implementation of Component<br/>
///     b) does not have a field of type Component
///     c) if Component is an abstract class, it overrides the method of Component
/// 3) Requirements for Base Decorator:
///     a) is an implementation of Component
///     b) is an abstract class
///     c) has a field of type Component
///     d) has a constructor with a parameter of type Component, which it passes to its field
///     e) calls the method of its field in the implementation of the method of Component<br/>
///         i) if Component is an abstract class, it overrides the method of Component
/// 4) Requirements for Concrete Decorator:
///     a) inherits from Base Decorator
///     b) calls the method of its parent in the implementation of the method of Component
///     c) has a function providing extra behaviour which it calls in the implementation of the method of Component<br/>
/// 5) Requirements for Client:
///     a) has created an object of the type ConcreteComponent
///     b) has created an object of the type ConcreteDecorator, to which it passes the ConcreteComponent
///     c) has called the method of ConcreteDecorator
/// </remarks>
internal class DecoratorRecognizer : IRecognizer
{
    /// <inheritdoc />
    public string Name => "Decorator";

    /// <inheritdoc />
    public Recognizer RecognizerType => Recognizer.Decorator;

    /// <inheritdoc />
    public IEnumerable<ICheck> Create()
    {
        //The Decorator utilizing an interface
        DecoratorRecognizerParent hasInterface = new DecoratorRecognizerWithInterface();
        //The Decorator utilizing an abstract class
        DecoratorRecognizerParent hasAbstractClass = new DecoratorRecognizerWithAbstractClass();

        yield return
            Any(
                Priority.Knockout,
                All(
                    Priority.Knockout,
                    hasInterface.Checks()
                ),
                All(
                    Priority.Knockout,
                    hasAbstractClass.Checks()
                )
            );
    }
}

/// <summary>
/// An abstract class defining the methods that are the parts of the Decorator recognizer.
/// It has implemented the methods which are the same in case of an interface or abstract class,
/// and it has defined the methods which are different in case of an interface or abstract class.
/// </summary>
abstract file class DecoratorRecognizerParent
{
    /// <summary>
    /// Makes a list of checks that form the requirements to be a Decorator recognizer.
    /// </summary>
    public ICheck[] Checks()
    {
        ICheck[] result = new ICheck[5];

        //Checks for requirement 1
        MethodCheck componentMethod = ComponentMethod();
        ICheck component = Component(componentMethod);

        //Checks for requirement 2
        ClassCheck concreteComponent = ConcreteComponent(component, componentMethod);

        //Checks for requirement 3
        FieldCheck baseDecoratorField = BaseDecoratorField(component);
        MethodCheck baseDecoratorMethod = BaseDecoratorMethod(componentMethod, baseDecoratorField);
        ClassCheck baseDecorator = BaseDecorator(component, baseDecoratorField, baseDecoratorMethod);

        //Checks for requirement 4
        MethodCheck concreteDecoratorExtraMethod = ConcreteDecoratorExtraMethod();
        ClassCheck concreteDecorator = ConcreteDecorator(baseDecorator, concreteDecoratorExtraMethod, baseDecoratorMethod);

        //Checks for requirement 5
        ClassCheck client = Client(componentMethod, concreteDecorator, concreteComponent);

        result[0] = component;
        result[1] = concreteComponent;
        result[2] = baseDecorator;
        result[3] = concreteDecorator;
        result[4] = client;

        return result;
    }

    /// <summary>
    /// Check for the <see cref="MethodCheck"/> of the Component of the Decorator pattern.
    /// Used to check that there is a <see cref="IMethod"/> adhering to the requirements of the Component method.
    /// </summary>
    /// <remarks>
    /// Checks part 1a of the requirements defined for a Decorator.
    /// </remarks>
    protected abstract MethodCheck ComponentMethod();

    /// <summary>
    /// Check for the of the Component of the Decorator pattern, which can be either a
    /// <see cref="ClassCheck"/> or an <see cref="InterfaceCheck"/>.
    /// Used to check that there is an <see cref="IEntity"/> adhering to the requirements of the Component.
    /// </summary>
    /// <remarks>
    /// Checks part 1 of the requirements defined for a Decorator.
    /// </remarks>
    protected abstract ICheck Component(MethodCheck componentMethod);

    /// <summary>
    /// Check for the <see cref="RelationCheck"/> of both the ConcreteComponent and ConcreteDecorator.
    /// Used to check that the found ConcreteDecorator and ConcreteComponent extend from Component.
    /// </summary>
    /// <remarks>
    /// Checks part 2a and 3a of the requirements defined for a Decorator.
    /// </remarks>
    protected abstract RelationCheck Extends(ICheck parent);

    /// <summary>
    /// Check for the method of ConcreteComponent of the Decorator pattern.
    /// Checks that there is an <see cref="IMethod"/> adhering to the requirements of the ConcreteComponent.
    /// </summary>
    /// <remarks>
    /// Checks part 2c of the requirements defined for a Decorator.
    /// </remarks>
    protected abstract MethodCheck ConcreteComponentMethod(ICheck component);

    /// <summary>
    /// Check for the ConcreteComponent of the Decorator pattern.
    /// Checks that there is an <see cref="IClass"/> adhering to the requirements of the ConcreteComponent.
    /// </summary>
    /// <remarks>
    /// Checks part 2 of the requirements defined for a Decorator.
    /// </remarks>
    protected ClassCheck ConcreteComponent(ICheck component, MethodCheck componentMethod) =>
            Class(
                Priority.Knockout,
                Extends(component),
                ConcreteComponentMethod(componentMethod),
                Not(
                    Priority.Knockout,
                    BaseDecoratorField(component)
                )
            );

    /// <summary>
    /// Check for the <see cref="FieldCheck"/> of the BaseDecorator of the Decorator pattern.
    /// Checks that there is an <see cref="IField"/> adhering to the requirements of the BaseDecorator.
    /// </summary>
    /// <remarks>
    /// Checks part 3c of the requirements defined for a Decorator.
    /// </remarks>
    protected FieldCheck BaseDecoratorField(ICheck component) =>
            Field(
                Priority.Knockout,
                Type(
                    Priority.Knockout,
                    (CheckBase)component
                )
            );

    /// <summary>
    /// Check for the <see cref="MethodCheck"/> of the BaseDecorator of the Decorator pattern.
    /// Checks that there is an <see cref="IMethod"/> adhering to the requirements of the BaseDecorator.
    /// </summary>
    /// <remarks>
    /// Checks part 3e of the requirements defined for a Decorator.
    /// </remarks>
    protected abstract MethodCheck BaseDecoratorMethod(MethodCheck componentMethod, FieldCheck baseDecoratorField);

    /// <summary>
    /// Check for the BaseDecorator of the Decorator pattern.
    /// Checks that there is an <see cref="IClass"/> adhering to the requirements of the BaseDecorator.
    /// </summary>
    /// <remarks>
    /// Checks part 3 of the requirements defined for a Decorator.
    /// </remarks>
    protected ClassCheck BaseDecorator(ICheck component, FieldCheck baseDecoratorField, MethodCheck baseDecoratorMethod) =>
            Class(
                Priority.Knockout,
                Modifiers(
                    Priority.High,
                    Modifier.Abstract
                ),
                Extends(component),
                baseDecoratorField,
                Constructor(
                    Priority.High,
                    Uses(
                        Priority.High,
                        baseDecoratorField
                    ),
                    Parameters(
                        Priority.High,
                        Type(
                            Priority.High,
                            (CheckBase)component
                        )
                    )
                ),
                baseDecoratorMethod
            );

    /// <summary>
    /// Check for the <see cref="MethodCheck"/> of the ConcreteDecorator of the Decorator pattern.
    /// Checks that there is an <see cref="IMethod"/> adhering to the requirements of the extra method of ConcreteDecorator.
    /// </summary>
    /// <remarks>
    /// Checks part 4c of the requirements defined for a Decorator.
    /// </remarks>
    protected MethodCheck ConcreteDecoratorExtraMethod() => Method(Priority.Mid);

    /// <summary>
    /// Check for the ConcreteDecorator of the Decorator pattern.
    /// Checks that there is an <see cref="IClass"/> adhering to the requirements of the ConcreteDecorator.
    /// </summary>
    /// <remarks>
    /// Checks part 4 of the requirements defined for a Decorator.
    /// </remarks>
    protected ClassCheck ConcreteDecorator(ClassCheck baseDecorator, MethodCheck concreteDecoratorExtraMethod, MethodCheck baseDecoratorMethod) =>
            Class(
                Priority.Knockout,
                Inherits(
                    Priority.Knockout,
                    baseDecorator
                ),
                concreteDecoratorExtraMethod,
                Method(
                    Priority.Knockout,
                    Overrides(
                        Priority.Knockout,
                        baseDecoratorMethod
                    ),
                    Uses(
                        Priority.Knockout,
                        baseDecoratorMethod
                    ),
                    Uses(
                        Priority.Mid,
                        concreteDecoratorExtraMethod
                    )
                )
            );

    /// <summary>
    /// Check for the Client of the Decorator pattern.
    /// Checks that there is an <see cref="IClass"/> adhering to the requirements of the Client.
    /// </summary>
    /// <remarks>
    /// Checks part 5 of the requirements defined for a Decorator.
    /// </remarks>
    protected ClassCheck Client(MethodCheck componentMethod, ClassCheck concreteDecorator, ClassCheck concreteComponent) =>
            Class(
                Priority.Low,
                Uses(
                    Priority.Low,
                    componentMethod
                ),
                Creates(
                    Priority.Low,
                    concreteDecorator
                ),
                Creates(
                    Priority.Low,
                    concreteComponent
                )
            );
    }

/// <summary>
/// A class implementing the parts of the Decorator recognizer in case it uses an abstract class.
/// </summary>
file class DecoratorRecognizerWithAbstractClass : DecoratorRecognizerParent
{
    /// <inheritdoc />
    protected override MethodCheck ComponentMethod() =>
            Method(
                Priority.Knockout,
                Modifiers(
                    Priority.Knockout,
                    Modifier.Abstract
                )
            );

    /// <inheritdoc />
    protected override ClassCheck Component(MethodCheck componentMethod) =>
            AbstractClass(
                Priority.Knockout,
                componentMethod
            );

    /// <inheritdoc />
    protected override RelationCheck Extends(ICheck parent) =>
            Inherits(
                Priority.Knockout,
                parent
            );

    /// <inheritdoc />
    protected override MethodCheck ConcreteComponentMethod(ICheck componentMethod) =>
            Method(
                Priority.Knockout,
                Overrides(
                    Priority.Knockout,
                    componentMethod
                )
            );

    /// <inheritdoc />
    protected override MethodCheck BaseDecoratorMethod(MethodCheck componentMethod, FieldCheck baseDecoratorField) =>
            Method(
                Priority.Knockout,
                Uses(
                    Priority.Knockout,
                    componentMethod
                ),
                Uses(
                    Priority.Knockout,
                    baseDecoratorField
                ),
                Overrides(
                    Priority.Knockout,
                    componentMethod
                )
            );
    }

/// <summary>
/// A class implementing the parts of the Decorator recognizer in case it uses an interface.
/// </summary>
file class DecoratorRecognizerWithInterface : DecoratorRecognizerParent
{
    /// <inheritdoc />
    protected override MethodCheck ComponentMethod() => Method(Priority.Knockout);

    /// <inheritdoc />
    protected override InterfaceCheck Component(MethodCheck componentMethod) =>
        Interface(
            Priority.Knockout,
            componentMethod
        );

    /// <inheritdoc />
    protected override RelationCheck Extends(ICheck parent) =>
            Implements(
                Priority.Knockout,
                parent
            );

    /// <inheritdoc />
    protected override MethodCheck ConcreteComponentMethod(ICheck component) => Method(Priority.Knockout);

    /// <inheritdoc />
    protected override MethodCheck BaseDecoratorMethod(MethodCheck componentMethod, FieldCheck baseDecoratorField) =>
            Method(
                Priority.Knockout,
                Uses(
                    Priority.Knockout,
                    componentMethod
                ),
                Uses(
                    Priority.Knockout,
                    baseDecoratorField
                )
            );
    }
