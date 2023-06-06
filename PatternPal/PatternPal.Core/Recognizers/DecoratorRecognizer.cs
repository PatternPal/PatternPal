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
internal class DecoratorRecognizer : IRecognizer
{
    /// <inheritdoc />
    public string Name => "Decorator";

    /// <inheritdoc />
    public Recognizer RecognizerType => Recognizer.Decorator;

    /// <summary>
    /// A method which creates a lot of <see cref="ICheck"/>s that each adheres to the requirements a decorator pattern needs to have implemented.
    /// It returns the requirements in a tree structure stated per class.
    /// </summary>
    public IEnumerable<ICheck> Create()
    {
        //The Decorator utilizing an interface
        DecoratorRecognizerParent hasInterface = new DecoratorRecognizerWithInterface();
        //The Decorator utilizing an abstract class
        DecoratorRecognizerParent hasAbstractClass = new DecoratorRecognizerWithAbstractClass();

        yield return
            Any(
                Priority.Knockout, //TODO welke prio moet dit zijn
                All(
                    Priority.Knockout, //TODO welke prio moet dit zijn
                    hasInterface.Checks()
                ),
                All(
                    Priority.Knockout, //TODO welke prio moet dit zijn
                    hasAbstractClass.Checks()
                )
            );

        /*MethodCheck componentMethod = Method(Priority.Knockout);

        //TODO add extra implementation for abstract class and put in Any
        InterfaceCheck component =
            Interface(
                Priority.Knockout,
                componentMethod
            );

        ClassCheck concreteComponent =
            Class(
                Priority.Knockout,
                Implements(
                    Priority.Knockout,
                    component
                ),
                Method(Priority.Knockout)
            );

        FieldCheck baseDecoratorField =
            Field(
                Priority.Knockout,
                Type(
                    Priority.Knockout,
                    component
                )
            );

        MethodCheck baseDecoratorMethod =
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

        ClassCheck baseDecorator =
            Class(
                Priority.Knockout,
                Modifiers(
                    Priority.High,
                    Modifier.Abstract
                ),
                Implements(
                    Priority.Knockout,
                    component
                ),
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
                            component
                        )
                    )
                ),
                baseDecoratorMethod
            );

        MethodCheck concreteDecoratorExtraMethod =
            Method(Priority.Mid);

        ClassCheck concreteDecorator =
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

        ClassCheck client =
            Class(
                Priority.Low,
                Uses(
                    Priority.Low,
                    component
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

        yield return component;
        yield return concreteComponent;
        yield return baseDecorator;
        yield return concreteDecorator;
        yield return client;*/
    }

    public List<IInstruction> GenerateStepsList()
    {
        throw new NotImplementedException();
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
    /// Makes a list of checks that form the requirements to be a Decorator recognizer:<br/>
    /// 1) Requirements for Component:
    ///     a) is an interface / abstract class
    ///     b) has declared a method
    ///         i) if the class is an abstract instead of an interface the method has to be an abstract method
    /// 2) Requirements for Concrete Component:
    ///     a) is an implementation of Component<br/>
    /// 3) Requirements for Base Decorator:
    ///     a) is an implementation of Component
    ///     b) is an abstract class
    ///     c) has a field of type Component
    ///     d) has a constructor with a parameter of type Component, which it passes to its field
    ///     e) calls the method of its field in the implementation of the method of Component<br/>
    /// 4) Requirements for Concrete Decorator:
    ///     a) inherits from Base Decorator
    ///     b) calls the method of its parent in the implementation of the method of Component
    ///     c) has a function providing extra behaviour which it calls in the implementation of the method of Component<br/>
    /// 5) Requirements for Client:
    ///     a) has created an object of the type ConcreteComponent
    ///     b) has created an object of the type ConcreteDecorator, to which it passes the ConcreteComponent
    ///     c) has called the method of ConcreteDecorator
    /// </summary>
    public ICheck[] Checks()
    {
        ICheck[] result = new ICheck[5];

        //Checks for 1
        MethodCheck componentMethod = ComponentMethod();
        ICheck component = Component(componentMethod);

        //Check for 2
        ClassCheck concreteComponent = ConcreteComponent(component);

        //Check for 3
        FieldCheck baseDecoratorField = BaseDecoratorField(component);
        MethodCheck baseDecoratorMethod = BaseDecoratorMethod(componentMethod, baseDecoratorField);
        ClassCheck baseDecorator = BaseDecorator(component, baseDecoratorField, baseDecoratorMethod);

        //Check for 4
        MethodCheck concreteDecoratorExtraMethod = ConcreteDecoratorExtraMethod();
        ClassCheck concreteDecorator = ConcreteDecorator(baseDecorator, concreteDecoratorExtraMethod, baseDecoratorMethod);

        //Check for 5
        ClassCheck client = Client(component, concreteDecorator, concreteComponent);

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
    protected abstract RelationCheck ExtendsFrom(ICheck parent);

    /// <summary>
    /// Check for the ConcreteComponent of the Decorator pattern.
    /// Checks that there is an <see cref="IClass"/> adhering to the requirements of the ConcreteComponent.
    /// </summary>
    /// <remarks>
    /// Checks part 2 of the requirements defined for a Decorator.
    /// </remarks>
    protected ClassCheck ConcreteComponent(ICheck component)
    {
        return
            Class(
                Priority.Knockout,
                ExtendsFrom(component),
                Method(Priority.Knockout)
            );
    }

    /// <summary>
    /// Check for the <see cref="FieldCheck"/> of the BaseDecorator of the Decorator pattern.
    /// Checks that there is an <see cref="IField"/> adhering to the requirements of the BaseDecorator.
    /// </summary>
    /// <remarks>
    /// Checks part 3c of the requirements defined for a Decorator.
    /// </remarks>
    protected FieldCheck BaseDecoratorField(ICheck component)
    {
        return 
            Field(
                Priority.Knockout,
                Type(
                    Priority.Knockout,
                    (CheckBase)component
                )
            );
    }

    /// <summary>
    /// Check for the <see cref="MethodCheck"/> of the BaseDecorator of the Decorator pattern.
    /// Checks that there is an <see cref="IMethod"/> adhering to the requirements of the BaseDecorator.
    /// </summary>
    /// <remarks>
    /// Checks part 3e of the requirements defined for a Decorator.
    /// </remarks>
    protected MethodCheck BaseDecoratorMethod(MethodCheck componentMethod, FieldCheck baseDecoratorField)
    {
        return
            Method(
                Priority.Knockout,
                    Uses(
                        Priority.Knockout,
                        componentMethod
                    ),
                    Uses(
                        Priority.Knockout,
                        baseDecoratorField //TODO check of dit werkt
                    )
            );
    }

    /// <summary>
    /// Check for the BaseDecorator of the Decorator pattern.
    /// Checks that there is an <see cref="IClass"/> adhering to the requirements of the BaseDecorator.
    /// </summary>
    /// <remarks>
    /// Checks part 3 of the requirements defined for a Decorator.
    /// </remarks>
    protected ClassCheck BaseDecorator(ICheck component, FieldCheck baseDecoratorField, MethodCheck baseDecoratorMethod)
    {
        return
            Class(
                Priority.Knockout,
                Modifiers(
                    Priority.High,
                    Modifier.Abstract
                ),
                ExtendsFrom(component),
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
    }

    /// <summary>
    /// Check for the <see cref="MethodCheck"/> of the ConcreteDecorator of the Decorator pattern.
    /// Checks that there is an <see cref="IMethod"/> adhering to the requirements of the extra method of ConcreteDecorator.
    /// </summary>
    /// <remarks>
    /// Checks part 4c of the requirements defined for a Decorator.
    /// </remarks>
    protected MethodCheck ConcreteDecoratorExtraMethod()
    {
        return Method(Priority.Mid);
    }

    /// <summary>
    /// Check for the ConcreteDecorator of the Decorator pattern.
    /// Checks that there is an <see cref="IClass"/> adhering to the requirements of the ConcreteDecorator.
    /// </summary>
    /// <remarks>
    /// Checks part 4 of the requirements defined for a Decorator.
    /// </remarks>
    protected ClassCheck ConcreteDecorator(ClassCheck baseDecorator, MethodCheck concreteDecoratorExtraMethod, MethodCheck baseDecoratorMethod)
    {
        return
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
    }

    /// <summary>
    /// Check for the Client of the Decorator pattern.
    /// Checks that there is an <see cref="IClass"/> adhering to the requirements of the Client.
    /// </summary>
    /// <remarks>
    /// Checks part 5 of the requirements defined for a Decorator.
    /// </remarks>
    protected ClassCheck Client(ICheck component, ClassCheck concreteDecorator, ClassCheck concreteComponent)
    {
        return
            Class(
                Priority.Low,
                Uses(
                    Priority.Low,
                    component
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
}

/// <summary>
/// A class implementing the parts of the Decorator recognizer in case it uses an abstract class.
/// </summary>
file class DecoratorRecognizerWithAbstractClass : DecoratorRecognizerParent
{
    /// <inheritdoc />
    protected override MethodCheck ComponentMethod()
    {
        return 
            Method(
                Priority.Knockout,
                Modifiers(
                    Priority.Knockout,
                    Modifier.Abstract
                )
            );
    }

    /// <inheritdoc />
    protected override ClassCheck Component(MethodCheck componentMethod)
    {
        return
            AbstractClass(
                Priority.Knockout,
                componentMethod
            );
    }

    /// <inheritdoc />
    protected override RelationCheck ExtendsFrom(ICheck parent)
    {
        return
            Inherits(
                Priority.Knockout,
                parent
            );
    }
}

/// <summary>
/// A class implementing the parts of the Decorator recognizer in case it uses an interface.
/// </summary>
file class DecoratorRecognizerWithInterface : DecoratorRecognizerParent
{
    /// <inheritdoc />
    protected override MethodCheck ComponentMethod()
    {
        return Method(Priority.Knockout);
    }

    /// <inheritdoc />
    protected override InterfaceCheck Component(MethodCheck componentMethod)
    {
        return Interface(
            Priority.Knockout,
            componentMethod
        );
    }

    /// <inheritdoc />
    protected override RelationCheck ExtendsFrom(ICheck parent)
    {
        return
            Implements(
                Priority.Knockout,
                parent
            );
    }
}
