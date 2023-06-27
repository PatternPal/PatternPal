#region

using PatternPal.SyntaxTree.Models;
using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers.Helper_Classes;

/// <summary>
/// The basis of the bridge pattern, it is used in the bridge interface checks creation and the bridge abstract class checks
/// </summary>
internal abstract class BridgeRecognizerParent
{
    // Implementation abstract class or interface requirement b)
    public MethodCheck ImplementationMethod { get; private set; }

    // Implementation abstract class or interface check
    public CheckBase ImplementationClass { get; private set; }

    // Abstraction class requirement a)
    public ICheck AbstractionFieldOrPropertyCheck { get; private set; }

    // Abstraction class sub-requirement a)
    public PropertyCheck ImplementationProperty { get; private set; }

    // Abstraction class requirement c)
    public MethodCheck AbstractionMethodUsesImplementationMethod { get; private set; }

    // Abstraction class check
    public ClassCheck AbstractionClass { get; private set; }

    // Concrete Implementation class check
    public ClassCheck ConcreteImplementationClass { get; private set; }

    // Client class requirement a)
    public ICheck ClientUsesMethod { get; private set; }

    // Client class check
    public ClassCheck ClientClass { get; private set; }

    /// <summary>
    /// A method that initializes the most important checks and sets the property of the associated check.
    /// </summary>
    public void Initialize()
    {
        // Implementation b)
        ImplementationMethod = HasMethodCheck();
        // Implementation class
        ImplementationClass = ImplementationCheckBase();

        // Abstraction a)
        AbstractionFieldOrPropertyCheck = HasFieldOrPropertyCheck(out PropertyCheck implementationProperty);
        // Abstraction sub-a)
        ImplementationProperty = implementationProperty;
        // Abstraction c)
        AbstractionMethodUsesImplementationMethod = HasMethodWithUseCheck();
        // Abstraction class
        AbstractionClass = Class(
            Priority.Knockout,
            "2. Abstraction Class",
            AbstractionFieldOrPropertyCheck,
            AbstractionMethodUsesImplementationMethod,
            SetAvailabilityFieldOrPropertyCheck(
                out ConstructorCheck setImplementationConstructor,
                out MethodCheck setImplementationMethod)
        );

        // Concrete Implementation class
        ConcreteImplementationClass = ConcreteImplementationCheck();

        // Client a)
        ClientUsesMethod = Uses(Priority.Mid, "5a. Calls a method in the Abstraction class.", AbstractionMethodUsesImplementationMethod);
        // Client class
        ClientClass = ClientClassCheck(
            ClientUsesMethod,
            ConcreteImplementationClass,
            ImplementationProperty,
            setImplementationConstructor,
            setImplementationMethod
        );
    }

    /// <summary>
    /// A method that creates a list of <see cref="ICheck"/>s that represent the basis of the Bridge pattern. Due to the
    /// a differing priority check some class checks are recreated compared to the checks used in the step by step mode.
    /// </summary>
    /// <returns>An array of <see cref="ICheck"/>s containing the five class checks that a bridge pattern holds.</returns>
    public ICheck[] Checks()
    {
        // requirements checks for Abstraction class
        // req. b
        MethodCheck methodInAbstraction = Method(Priority.Knockout, "2b. Has a method.");

        // req. d
        ICheck setAvailabilityFieldOrProperty = SetAvailabilityFieldOrPropertyCheck(
            out ConstructorCheck setImplementationConstructor, out MethodCheck setImplementationMethod);

        ClassCheck abstractionCheck = Class(
            Priority.Knockout,
            "2. Abstraction Class",
            methodInAbstraction,
            AbstractionFieldOrPropertyCheck,
            AbstractionMethodUsesImplementationMethod,
            setAvailabilityFieldOrProperty
        );

        // Refined Abstraction class check
        ClassCheck refinedAbstractionCheck = RefinedAbstractionClassCheck(abstractionCheck);

        // 
        ClassCheck clientCheck = ClientClassCheck(
            ClientUsesMethod,
            ConcreteImplementationClass,
            ImplementationProperty,
            setImplementationConstructor,
            setImplementationMethod
        );

        return new ICheck[]
        {
            ImplementationClass , abstractionCheck, ConcreteImplementationClass, refinedAbstractionCheck, clientCheck
        };
        
    }

    /// <summary>
    /// A <see cref="MethodCheck"/> which will check the existence of a method.
    /// </summary>
    /// <returns>A <see cref="MethodCheck"/> depending on the type of the class it exists in. </returns>
    public abstract MethodCheck HasMethodCheck();

    /// <summary>
    /// A <see cref="CheckBase"/> which holds either a <see cref="InterfaceCheck"/> or a <see cref="ClassCheck"/> depending
    /// on the content of the override.
    /// </summary>
    /// <returns>A <see cref="CheckBase"/> with a <see cref="MethodCheck"/>.</returns>
    public abstract CheckBase ImplementationCheckBase();

    /// <summary>
    /// A <see cref="ICheck"/> which checks for the existence of either a field or a property with type
    /// <see cref="ImplementationClass"/> and has either the <see cref="Modifier.Private"/> or
    /// the <see cref="Modifier.Protected"/> modifier.
    /// </summary>
    /// <param name="implementationProperty">A <see cref="PropertyCheck"/> that is created that is verified 
    /// to the <see cref="FieldCheck"/> in the <see cref="ICheck"/></param>
    /// <returns>A <see cref="ICheck"/> that checks for the existence of either a field or a property. </returns>
    public ICheck HasFieldOrPropertyCheck(out PropertyCheck implementationProperty)
    {
        implementationProperty = Property(Priority.Knockout,
            Type(Priority.Knockout, 
                ImplementationClass
            ),
            Any(Priority.Knockout,
                Modifiers(Priority.Knockout, 
                    Modifier.Private
                ),
                Modifiers(Priority.Knockout, 
                    Modifier.Protected
                )
            )
        );

        return Any(
            Priority.Knockout,
            "2a. Has a private or protected field or property with the Implementation type.",
            Field(
                Priority.Knockout,
                Type( Priority.Knockout, 
                    ImplementationClass
                ),
                Any(Priority.Knockout, 
                    Modifiers(Priority.Knockout, 
                        Modifier.Private
                    ), 
                    Modifiers(Priority.Knockout, 
                        Modifier.Protected
                    )
                )
            ),
            implementationProperty
        );
    }

    /// <summary>
    /// A <see cref="MethodCheck"/> which checks for a method that uses an method instance that
    /// adheres to the <see cref="ImplementationMethod"/> <see cref="MethodCheck"/>.
    /// <returns>A <see cref="MethodCheck"/> that checks for the uses relation with <see cref="ImplementationMethod"/>.</returns>
    /// </summary>
    public MethodCheck HasMethodWithUseCheck()
    {
        return Method(
            Priority.High,
            "2c. Has a method that calls a method in the Implementation interface or abstract class.",
            Uses(
                Priority.High,
                ImplementationMethod
            )
        );
    }

    /// <summary>
    /// A method that creates a <see cref="NodeCheck{TNode}"/> to verify the existence of
    /// either the <paramref name="setImplementationConstructor"/> check or the
    /// <paramref name="setImplementationMethod" /> check or the <see cref="ImplementationProperty"/> check.
    /// </summary>
    /// <param name="setImplementationConstructor">A <see cref="ConstructorCheck"/> that checks whether there is a
    /// not private or protected constructor with a <see cref="ImplementationClass"/> as type.</param>
    /// <param name="setImplementationMethod">A <see cref="MethodCheck"/> that checks whether there is a
    /// not private and not protected method with a <see cref="ImplementationClass"/> as type.</param>
    /// <returns>A <see cref="ICheck"/> that holds a check that verifies the existence of an instance that adheres to
    /// the <paramref name="setImplementationConstructor"/> check or the <paramref name="setImplementationMethod" /> check
    /// or the <see cref="ImplementationProperty"/> check.</returns>
    internal ICheck SetAvailabilityFieldOrPropertyCheck(out ConstructorCheck setImplementationConstructor, out MethodCheck setImplementationMethod)
    {
        ICheck[] properties = new ICheck[]
        {
            Not(Priority.Mid,
                Modifiers(Priority.Mid,
                    Modifier.Private
                )
            ),            
            Not(Priority.Mid,
                Modifiers(Priority.Mid,
                    Modifier.Protected
                )
            ),
            Parameters(
                Priority.Mid,
                Type(Priority.Mid, 
                    ImplementationClass
                )
            )
        };

        setImplementationConstructor = Constructor(Priority.Mid, properties);

        setImplementationMethod = Method(Priority.Mid, properties);

        return Any(
            Priority.Mid,
            "2d. Has either a property as described in the first requirement, or a constructor or a method with a parameter that has the Implementation type and uses the field as described in the first requirement.",
            ImplementationProperty,
            setImplementationConstructor,
            setImplementationMethod
        );
    }

    /// <summary>
    /// A <see cref="ClassCheck"/> that checks the requirements of the Concrete Implementation class, which holds a
    /// <see cref="RelationCheck"/> to check the relation with the Implementation class or interface
    /// </summary>
    /// <returns>A <see cref="ClassCheck"/> that checks for a class that adheres to the requirements of an instance of
    /// the concreteImplementation</returns>
    public abstract ClassCheck ConcreteImplementationCheck();

    /// <summary>
    /// A <see cref="ClassCheck"/> that checks for requirements of the Refined Abstraction class
    /// </summary>
    /// <param name="abstractionCheck">A <see cref="ClassCheck"/> that adheres to the requirements of the Abstraction Class check</param>
    /// <returns>A <see cref="ClassCheck"/> that checks for the existence of a <see cref="MethodCheck"/> and a <see cref="RelationType.Extends"/> relation
    /// with the <paramref name="abstractionCheck"/>.</returns>
    public ClassCheck RefinedAbstractionClassCheck(ClassCheck abstractionCheck)
    {
        return Class(
            Priority.Knockout,
            "4. Refined Abstraction Class",
            Inherits(
                Priority.Knockout, 
                "4a. Inherits from the Abstraction class.",
                abstractionCheck
            ),
            Method(
                Priority.High, 
                "4b. Has a method."
            )
        );
    }

    /// <summary>
    /// A method to create a <see cref="ClassCheck"/> that verifies requirements adhering to the Client class.
    /// </summary>
    /// <param name="methodInAbstractionWithUse">A <see cref="ICheck"/> that checks for the uses of a method in the Abstraction class.</param>
    /// <param name="concreteImplementationCheck">A <see cref="ClassCheck"/> that checks the requirements of the Implementation entity check.</param>
    /// <param name="implementationProperty">A <see cref="PropertyCheck"/> that could be in the Abstraction class.</param>
    /// <param name="setImplementationConstructor">A <see cref="ConstructorCheck"/> that could be in the Abstraction class.</param>
    /// <param name="setImplementationMethod">A <see cref="MethodCheck"/> that could be in the Abstraction class.</param>
    /// <returns>A <see cref="ClassCheck"/> that checks for the existence of a class adhering to all te requirements for the Client class</returns>
    public ClassCheck ClientClassCheck(
        ICheck methodInAbstractionWithUse, ClassCheck concreteImplementationCheck, 
        PropertyCheck implementationProperty, ConstructorCheck setImplementationConstructor, 
        MethodCheck setImplementationMethod)
    {
        return Class(
            Priority.Low,
            "5. Client Class",
            methodInAbstractionWithUse,
            Creates(Priority.Low, "5b. Creates a Concrete Implementation instance.", concreteImplementationCheck),
            SetsImplementationFieldCheck(implementationProperty, setImplementationConstructor, setImplementationMethod)
        );

    }

    /// <summary>
    /// A method that creates a check that checks whether there is a <see cref="RelationType.Uses"/> relation with either <paramref name="implementationProperty"/>
    /// or a <paramref name="setImplementationConstructor"/> or a <paramref name="setImplementationMethod"/>.
    /// </summary>
    /// <param name="implementationProperty">The <see cref="PropertyCheck"/> of a property in the Implementation class that could be used.</param>
    /// <param name="setImplementationConstructor">The <see cref="ConstructorCheck"/> of a constructor in the Implementation class that could be used.</param>
    /// <param name="setImplementationMethod">The <see cref="MethodCheck"/> of a method in the Implementation class that could be used</param>
    /// <returns></returns>
    internal ICheck SetsImplementationFieldCheck(PropertyCheck implementationProperty, ConstructorCheck setImplementationConstructor, MethodCheck setImplementationMethod)
    {
        return Any(
            Priority.Low,
            "5c. Sets the field or property in Abstraction using a constructor, method or property.",
            Uses(Priority.Low, 
                setImplementationConstructor
            ),
            Uses(Priority.Low, 
                setImplementationMethod
            ),
            Uses(Priority.Low, 
                implementationProperty
            )
        );
    }

}
