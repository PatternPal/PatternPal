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
/// Requirements to fulfill the pattern:<br/>
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
/// </remarks>
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
        DecoratorRecognizerParent hasInterface = new DecoratorRecognizerWithInterface();
        DecoratorRecognizerParent hasAbstractClass = new DecoratorRecognizerWithAbstractClass();

        return
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

abstract file class DecoratorRecognizerParent
{
    public ICheck[] Checks()
    {
        ICheck[] result = new ICheck[5];

        //Check Client interface c, ci
        MethodCheck componentMethod = ComponentMethod();

        //Check Client interface a
        ICheck component = Component(componentMethod);

        //Helps check Client b
        ClassCheck concreteComponent = ConcreteComponent(component);

        //Check Concrete Service a
        FieldCheck baseDecoratorField = BaseDecoratorField(component);

        //Check Adapter a, Client interface b
        MethodCheck baseDecoratorMethod = BaseDecoratorMethod(componentMethod, baseDecoratorField);

        //Check Adapter b
        ClassCheck baseDecorator = BaseDecorator(component, baseDecoratorField, baseDecoratorMethod);

        //Check Adapter c
        MethodCheck concreteDecoratorExtraMethod = ConcreteDecoratorExtraMethod();

        //Check Adapter e, Service b
        ClassCheck concreteDecorator =
            ConcreteDecorator(baseDecorator, concreteDecoratorExtraMethod, baseDecoratorMethod);

        //Helps Client b
        ClassCheck client = Client(component, concreteDecorator, concreteComponent);

        result[0] = component;

        result[1] = concreteComponent;

        result[2] = baseDecorator;

        result[3] = concreteDecorator;

        result[4] = client;

        return result;
    }
    protected abstract MethodCheck ComponentMethod();

    protected abstract ICheck Component(MethodCheck componentMethod);

    protected abstract RelationCheck ExtendsFrom(ICheck parent);

    protected ClassCheck ConcreteComponent(ICheck component)
    {
        return
            Class(
                Priority.Knockout,
                ExtendsFrom(component),
                Method(Priority.Knockout)
            );
    }

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

    protected MethodCheck ConcreteDecoratorExtraMethod()
    {
        return Method(Priority.Mid);
    }

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

file class DecoratorRecognizerWithAbstractClass : DecoratorRecognizerParent
{
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

    protected override ClassCheck Component(MethodCheck componentMethod)
    {
        return
            AbstractClass(
                Priority.Knockout,
                componentMethod
            );
    }

    protected override RelationCheck ExtendsFrom(ICheck parent)
    {
        return
            Inherits(
                Priority.Knockout,
                parent
            );
    }
}

file class DecoratorRecognizerWithInterface : DecoratorRecognizerParent
{
    protected override MethodCheck ComponentMethod()
    {
        return Method(Priority.Knockout);
    }

    protected override InterfaceCheck Component(MethodCheck componentMethod)
    {
        return Interface(
            Priority.Knockout,
            componentMethod
        );
    }

    protected override RelationCheck ExtendsFrom(ICheck parent)
    {
        return
            Implements(
                Priority.Knockout,
                parent
            );
    }
}
