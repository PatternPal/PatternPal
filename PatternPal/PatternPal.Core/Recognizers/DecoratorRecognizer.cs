#region

using PatternPal.Core.StepByStep;
using PatternPal.SyntaxTree.Models;

using PatternPal.Core.StepByStep.Resources.Instructions;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided files or project implements the decorator pattern
/// </summary>
/// <remarks>
///     Requirements for Component: <br/>
///         a) is an interface / abstract class <br/>
///         b) has declared a method <br/>
///             i) if the class is an abstract class instead of an interface the method has to be an abstract method <br/>
///     Requirements for Concrete Component: <br/>
///         a) is an implementation of Component <br/>
///         b) does not have a field of type Component <br/>
///         c) if Component is an abstract class, it overrides the method of Component <br/>
///     Requirements for Base Decorator: <br/>
///         a) is an implementation of Component <br/>
///         b) is an abstract class <br/>
///         c) has a field of type Component <br/>
///         d) has a constructor with a parameter of type Component, which it passes to its field <br/>
///         e) calls the method of its field in the implementation of the method of Component <br/>
///             i) if Component is an abstract class, it overrides the method of Component <br/>
///     Requirements for Concrete Decorator: <br/>
///         a) inherits from Base Decorator <br/>
///         b) calls the method of its parent in the implementation of the method of Component <br/>
///         c) has a function providing extra behaviour which it calls in the implementation of the method of Component <br/>
///     Requirements for Client: <br/>
///         a) has created an object of the type ConcreteComponent <br/>
///         b) has created an object of the type ConcreteDecorator, to which it passes the ConcreteComponent <br/>
///         c) has called the method of ConcreteDecorator <br/>
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

        //Checks for Component entity
        MethodCheck componentMethod = ComponentMethod();
        ICheck component = Component(componentMethod);

        //Checks for Concrete Component entity
        ClassCheck concreteComponent = ConcreteComponent(component, componentMethod);

        //Checks for Base Decorator entity
        FieldCheck baseDecoratorField = BaseDecoratorField(component);
        MethodCheck baseDecoratorMethod = BaseDecoratorMethod(componentMethod, baseDecoratorField);
        ConstructorCheck baseDecoratorConstructor = BaseDecoratorConstructor(baseDecoratorField, component);
        ClassCheck baseDecorator = BaseDecorator(component, baseDecoratorField, baseDecoratorConstructor, baseDecoratorMethod);

        //Checks for Concrete Decorator entity
        MethodCheck concreteDecoratorExtraMethod = ConcreteDecoratorExtraMethod(componentMethod);
        MethodCheck concreteDecoratorMethod = ConcreteDecoratorMethod(baseDecoratorMethod, concreteDecoratorExtraMethod);
        ClassCheck concreteDecorator = ConcreteDecorator(baseDecorator, concreteDecoratorExtraMethod, concreteDecoratorMethod);

        //Checks for requirement 5
        ClassCheck client = Client(componentMethod, baseDecoratorMethod, concreteDecoratorMethod, concreteDecorator, concreteComponent);

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
    protected abstract RelationCheck Extends(ICheck parent, string ? requirement = null);

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
                "2. Concrete Component Class",
                Extends(component, "2a. Is an implementation of the component."),
                ConcreteComponentMethod(componentMethod),
                Not(
                    Priority.Knockout,
                    "2b. Does not have a field of type Component.",
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
                "3c. Has a field of type Component.",
                Type(
                    Priority.Knockout,
                    (CheckBase)component
                ),
                Modifiers(
                    Priority.High,
                    Modifier.Private
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
    /// Check for the <see cref="ConstructorCheck"/> of the BaseDecorator of the Decorator pattern.
    /// Checks that there is an <see cref="IConstructor"/> adhering to the requirements of the BaseDecorator.
    /// </summary>
    /// <remarks>
    /// Checks part 3d of the requirements defined for a Decorator.
    /// </remarks>
    protected ConstructorCheck BaseDecoratorConstructor(FieldCheck baseDecoratorField, ICheck component) =>
        Constructor(
            Priority.High,
            "3d. Has a constructor with a parameter of type Component, which it passes to its field.",
            Not(
                Priority.High,
                Modifiers(
                    Priority.High,
                    Modifier.Private
                )
            ),
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
        );

    /// <summary>
    /// Check for the BaseDecorator of the Decorator pattern.
    /// Checks that there is an <see cref="IClass"/> adhering to the requirements of the BaseDecorator.
    /// </summary>
    /// <remarks>
    /// Checks part 3 of the requirements defined for a Decorator.
    /// </remarks>
    protected ClassCheck BaseDecorator(ICheck component, FieldCheck baseDecoratorField, ConstructorCheck baseDecoratorConstructor, MethodCheck baseDecoratorMethod) =>
        Class(
            Priority.Knockout,
            "3. Base Decorator Class",
            Modifiers(
                Priority.High,
                "3b. Is an abstract class.",
                Modifier.Abstract
            ),
            Extends(component, "3a. Is an implementation of the Component."),
            baseDecoratorField,
            baseDecoratorConstructor,
            baseDecoratorMethod
        );

    /// <summary>
    /// Check for the extra <see cref="MethodCheck"/> of the ConcreteDecorator of the Decorator pattern.
    /// Checks that there is an <see cref="IMethod"/> adhering to the requirements of the extra method of ConcreteDecorator.
    /// </summary>
    /// <remarks>
    /// Checks part 4c of the requirements defined for a Decorator.
    /// </remarks>
    protected MethodCheck ConcreteDecoratorExtraMethod(MethodCheck componentMethod) => Method(
        Priority.Mid,
        "4c. Has a function providing extra behaviour which it calls in the implementation of the method of Component.",
        Not(
            Priority.Low,
            Uses(
                Priority.Low,
                componentMethod
            )
        ));

    /// <summary>
    /// Check for the <see cref="MethodCheck"/> of the ConcreteDecorator of the Decorator pattern.
    /// Checks that there is an <see cref="IMethod"/> adhering to the requirements of the method of ConcreteDecorator.
    /// </summary>
    /// <remarks>
    /// Checks part 4b and 4c of the requirements defined for a Decorator.
    /// </remarks>
    protected MethodCheck ConcreteDecoratorMethod(MethodCheck baseDecoratorMethod,
        MethodCheck concreteDecoratorExtraMethod) =>
        Method(
            Priority.Knockout,
            "4b. Calls the method of its parent in the implementation of the method of the Component.",
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
        );

    /// <summary>
    /// Check for the ConcreteDecorator of the Decorator pattern.
    /// Checks that there is an <see cref="IClass"/> adhering to the requirements of the ConcreteDecorator.
    /// </summary>
    /// <remarks>
    /// Checks part 4 of the requirements defined for a Decorator.
    /// </remarks>
    protected ClassCheck ConcreteDecorator(ClassCheck baseDecorator, MethodCheck concreteDecoratorExtraMethod, MethodCheck concreteDecoratorMethod) =>
        Class(
            Priority.Knockout,
            "4. Concrete Decorator Class",
            Inherits(
                Priority.Knockout,
                "4a. Inherits from Base Decorator.",
                baseDecorator
            ),
            concreteDecoratorExtraMethod,
            concreteDecoratorMethod
        );

    /// <summary>
    /// Check for the Client of the Decorator pattern.
    /// Checks that there is an <see cref="IClass"/> adhering to the requirements of the Client.
    /// </summary>
    /// <remarks>
    /// Checks part 5 of the requirements defined for a Decorator.
    /// </remarks>
    protected ClassCheck Client(MethodCheck componentMethod, MethodCheck baseDecoratorMethod, MethodCheck concreteDecoratorMethod, ClassCheck concreteDecorator, ClassCheck concreteComponent) =>
        Class(
            Priority.Low,
            "5. Client Class",
            Any(
                Priority.Low,
                "5c. Has called the method of ConcreteDecorator.",
                Uses(
                    Priority.Low,
                    componentMethod
                ),
                Uses(
                    Priority.Low,
                    baseDecoratorMethod
                ),
                Uses(
                    Priority.Low,
                    concreteDecoratorMethod
                )
            ),
            Creates(
                Priority.Low,
                "5b. Has created an object of the type ConcreteDecorator, to which it passes the ConcreteComponent.",
                concreteDecorator
            ),
            Creates(
                Priority.Low,
                "5a. Has created an object of the type ConcreteComponent.",
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
                "has declared a method.",
                Modifiers(
                    Priority.Knockout,
                    "if the class is an abstract class instead of an interface the method has to be an abstract method.",
                    Modifier.Abstract
                )
            );

    /// <inheritdoc />
    protected override ClassCheck Component(MethodCheck componentMethod) =>
            AbstractClass(
                Priority.Knockout,
                "1. Component Class",
                componentMethod
            );

    /// <inheritdoc />
    protected override RelationCheck Extends(ICheck parent, string ? requirement = null) =>
            Inherits(
                Priority.Knockout,
                requirement,
                parent
            );

    /// <inheritdoc />
    protected override MethodCheck ConcreteComponentMethod(ICheck componentMethod) =>
            Method(
                Priority.Knockout,
                "2c. If the Component is an abstract class, it overrides the method of the Component.",
                Overrides(
                    Priority.Knockout,
                    componentMethod
                )
            );

    /// <inheritdoc />
    protected override MethodCheck BaseDecoratorMethod(MethodCheck componentMethod, FieldCheck baseDecoratorField) =>
            Method(
                Priority.Knockout,
                "3c. There is a method which overrides a method of the Component, and uses a method of the instance saved in its field.",
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
file class DecoratorRecognizerWithInterface : DecoratorRecognizerParent, IStepByStepRecognizer
{
    /// <inheritdoc />
    protected override MethodCheck ComponentMethod() => Method(
        Priority.Knockout, 
        "1a. Has declared a method.");

    /// <inheritdoc />
    protected override InterfaceCheck Component(MethodCheck componentMethod) =>
        Interface(
            Priority.Knockout,
            "1. Component Interface",
            componentMethod
        );

    /// <inheritdoc />
    protected override RelationCheck Extends(ICheck parent, string ? requirement = null) =>
            Implements(
                Priority.Knockout,
                requirement,
                parent
            );

    /// <inheritdoc />
    protected override MethodCheck ConcreteComponentMethod(ICheck component) => Method(Priority.Knockout);

    /// <inheritdoc />
    protected override MethodCheck BaseDecoratorMethod(MethodCheck componentMethod, FieldCheck baseDecoratorField) =>
            Method(
                Priority.Knockout,
                "3e. There is a method which uses a method of the instance saved in its field.",
                Modifiers(
                    Priority.Knockout,
                    Modifier.Virtual
                ),
                Uses(
                    Priority.Knockout,
                    componentMethod
                ),
                Uses(
                    Priority.Knockout,
                    baseDecoratorField
                )
            );

    public string Name => "Decorator with interface";
    public Recognizer RecognizerType => Recognizer.Decorator;
    public List<IInstruction> GenerateStepsList()
    {
        List<IInstruction> generateStepsList = new();

        MethodCheck componentMethod = ComponentMethod();
        ICheck component = Component(componentMethod);

        generateStepsList.Add(
            new SimpleInstruction(
                DecoratorInstructions.Step1,
                DecoratorInstructions.Explanation1,
                new List<ICheck> { component }));

        ClassCheck concreteComponent = ConcreteComponent(component, componentMethod);

        generateStepsList.Add(
            new SimpleInstruction(
                DecoratorInstructions.Step2,
                DecoratorInstructions.Explanation2,
                new List<ICheck>{
                    component,
                    concreteComponent}));

        FieldCheck baseDecoratorField = BaseDecoratorField(component);

        generateStepsList.Add(
            new SimpleInstruction(
                DecoratorInstructions.Step3,
                DecoratorInstructions.Explanation3,
                new List<ICheck>
                {
                    component,
                    concreteComponent,
                    AbstractClass(
                        Priority.Knockout,
                        Extends(component),
                        baseDecoratorField,
                        Method(
                            Priority.Knockout,
                            Modifiers(
                                Priority.Knockout,
                                Modifier.Virtual
                            )
                        )
                    )
                })
            );

        ConstructorCheck baseDecoratorConstructor = BaseDecoratorConstructor(baseDecoratorField, component);
        generateStepsList.Add(
            new SimpleInstruction(
                DecoratorInstructions.Step4,
                DecoratorInstructions.Explanation4,
                new List<ICheck>
                {
                    component,
                    concreteComponent,
                    AbstractClass(
                        Priority.Knockout,
                        Extends(component),
                        baseDecoratorField,
                        baseDecoratorConstructor,
                        Method(
                            Priority.Knockout,
                            Modifiers(
                                Priority.Knockout,
                                Modifier.Virtual
                            )
                        )
                    )
                })
        );

        MethodCheck baseDecoratorMethod = BaseDecoratorMethod(componentMethod, baseDecoratorField);
        ClassCheck baseDecorator =
            BaseDecorator(component, baseDecoratorField, baseDecoratorConstructor, baseDecoratorMethod);
        generateStepsList.Add(
            new SimpleInstruction(
                DecoratorInstructions.Step5,
                DecoratorInstructions.Explanation5,
                new List<ICheck>
                {
                    component,
                    concreteComponent,
                    baseDecorator
                })
        );


        generateStepsList.Add(
            new SimpleInstruction(
                DecoratorInstructions.Step6,
                DecoratorInstructions.Explanation6,
                new List<ICheck>
                {
                    component,
                    concreteComponent,
                    baseDecorator,
                    Class(
                        Priority.Knockout,
                        Inherits(
                            Priority.Knockout,
                            baseDecorator
                        ),
                        Method(
                            Priority.Knockout,
                            Overrides(
                                Priority.Knockout,
                                baseDecoratorMethod
                            ),
                            Uses(
                                Priority.Knockout,
                                baseDecoratorMethod
                            )
                        )
                    )
                }));

        MethodCheck concreteDecoratorExtraMethod = ConcreteDecoratorExtraMethod(componentMethod);
        MethodCheck concreteDecoratorMethod =
            ConcreteDecoratorMethod(baseDecoratorMethod, concreteDecoratorExtraMethod);
        ClassCheck concreteDecorator =
            ConcreteDecorator(baseDecorator, concreteDecoratorExtraMethod, concreteDecoratorMethod);

        generateStepsList.Add(
            new SimpleInstruction(
                DecoratorInstructions.Step7,
                DecoratorInstructions.Explanation7,
                new List<ICheck>
                {
                    component,
                    concreteComponent,
                    baseDecorator,
                    concreteDecorator
                })
        );

        ClassCheck client = Client(componentMethod, baseDecoratorMethod, concreteDecoratorMethod, concreteDecorator, concreteComponent);

        generateStepsList.Add(
            new SimpleInstruction(
                DecoratorInstructions.Step8,
                DecoratorInstructions.Explanation8,
                new List<ICheck>
                {
                    component,
                    concreteComponent,
                    baseDecorator,
                    concreteDecorator,
                    client
                })
        );

        return generateStepsList;
    }
}
