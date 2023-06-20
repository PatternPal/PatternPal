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
    /// <summary>
    /// A method that creates a list of <see cref="ICheck"/>s that represent the basis of the Bridge pattern.  
    /// </summary>
    /// <returns>An array of <see cref="ICheck"/>s containing the five class checks that a bridge pattern holds.</returns>
    public ICheck[] Checks()
    {
        // requirements checks for Implementation 
        // req. b
        MethodCheck methodInImplementation = HasMethodCheck();

        // req. a
        CheckBase implementationCheck = HasInterfaceOrAbstractClassWithMethodCheck(methodInImplementation);

        // requirements checks for Abstraction class
        // req. a
        ICheck fieldOrProperty = HasFieldOrPropertyCheck(out PropertyCheck implementationProperty, implementationCheck);

        // req. b
        MethodCheck methodInAbstraction = Method(Priority.Knockout, "2b. Has a method.");

        // req. c
        MethodCheck methodInAbstractionWithUse = HasMethodWithUseCheck(methodInImplementation);

        // req. d
        ICheck setAvailabilityFieldOrProperty = SetAvailabilityFieldOrPropertyCheck(
            out ConstructorCheck setImplementationConstructor, out MethodCheck setImplementationMethod, 
            implementationCheck, implementationProperty);

        ClassCheck abstractionCheck = Class(Priority.High,
            "2. Abstraction class.",
            fieldOrProperty, 
            methodInAbstraction, 
            methodInAbstractionWithUse, 
            setAvailabilityFieldOrProperty);

        // requirements checks for Concrete Implementations
        // req. a & b 
        ClassCheck concreteImplementationCheck = ConcreteImplementationCheck(implementationCheck, methodInImplementation);

        // requirements checks for Refined Abstraction
        // req. a
        ICheck inheritsFromAbstraction = Inherits(Priority.Knockout,"4a. Inherits from the Abstraction class.", abstractionCheck);

        // req. b
        ICheck methodInRefinedAbstraction = Method(Priority.High, "4b. Has a method.");

        ClassCheck refinedAbstractionCheck = Class(Priority.High,"4. Is the Refined Abstraction class.", inheritsFromAbstraction, methodInRefinedAbstraction);

        // requirements checks for Client
        // req. a 
        MethodCheck methodUseInAbstraction = Method(Priority.Mid, "5a. Has a method that calls a method in the Abstraction class.", Uses(Priority.Mid, methodInAbstractionWithUse));

        // req. c
        ICheck setsImplementationField = SetsImplementationFieldCheck(implementationProperty, setImplementationConstructor, setImplementationMethod);

        
        ClassCheck clientCheck = Class( 
            Priority.Low,
            "5.Is the Client class.",
            methodUseInAbstraction,
            Creates(Priority.Low, "5b. Creates a Concrete Implementation instance.", concreteImplementationCheck), 
            setsImplementationField );

        return new ICheck[]
        {
            implementationCheck , abstractionCheck, concreteImplementationCheck, refinedAbstractionCheck, clientCheck
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
    /// <param name="methodInImplementation">The method that the class or interface should have. </param>
    /// <returns>A <see cref="CheckBase"/> with a <see cref="MethodCheck"/>.</returns>
    public abstract CheckBase HasInterfaceOrAbstractClassWithMethodCheck(MethodCheck methodInImplementation);

    /// <summary>
    /// A <see cref="ICheck"/> which checks for the existence of either a field or a property with type
    /// <paramref name="implementationCheck"/> and has either the <see cref="Modifier.Private"/> or
    /// the <see cref="Modifier.Protected"/> modifier.
    /// </summary>
    /// <param name="implementationCheck"> A <see cref="CheckBase"/> that either holds a <see cref="InterfaceCheck"/>
    /// or a <see cref="ClassCheck"/>.</param>
    /// <returns>A <see cref="ICheck"/> that checks for the existence of either a field or a property. </returns>
    public ICheck HasFieldOrPropertyCheck(out PropertyCheck implementationProperty, CheckBase implementationCheck)
    {
        implementationProperty = Property(Priority.Knockout,
            Type(Priority.Knockout, 
                implementationCheck
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
                    implementationCheck
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
    /// adheres to the <paramref name="methodInImplementation"/> <see cref="MethodCheck"/>.
    /// </summary>
    /// <param name="methodInImplementation"> A <see cref="MethodCheck"/> that is declared in the Implementation class or interface </param>
    /// <returns>A <see cref="MethodCheck"/> that checks for the uses relation with <paramref name="methodInImplementation"/>.</returns>
    public MethodCheck HasMethodWithUseCheck(MethodCheck methodInImplementation)
    {
        return Method(
            Priority.High,
            "2c. Has a method that calls a method in the Implementation interface or abstract class.",
            Uses(
                Priority.High,
                methodInImplementation
            )
        );
    }

    /// <summary>
    /// A method that creates a <see cref="NodeCheck{TNode}"/> to verify the existence of
    /// either the <paramref name="setImplementationConstructor"/> check or the
    /// <paramref name="setImplementationMethod" /> check or the <paramref name="implementationProperty"/> check.
    /// </summary>
    /// <param name="setImplementationConstructor">A <see cref="ConstructorCheck"/> that checks whether there is a
    /// not private or protected constructor with a <paramref name="implementationCheck"/> as type.</param>
    /// <param name="setImplementationMethod">A <see cref="MethodCheck"/> that checks whether there is a
    /// not private and not protected method with a <paramref name="implementationCheck"/> as type.</param>
    /// <param name="implementationCheck">A <see cref="CheckBase"/> that the instances the other checks verify have to have as type.</param>
    /// <param name="implementationProperty">A <see cref="PropertyCheck"/> that checks the existence of a property of
    /// <paramref name="implementationCheck"/></param>
    /// <returns>A <see cref="ICheck"/> that holds a check that verifies the existence of an instance that adheres to
    /// the <paramref name="setImplementationConstructor"/> check or the <paramref name="setImplementationMethod" /> check
    /// or the <paramref name="implementationProperty"/> check.</returns>
    internal ICheck SetAvailabilityFieldOrPropertyCheck(out ConstructorCheck setImplementationConstructor, out MethodCheck setImplementationMethod,
        CheckBase implementationCheck, PropertyCheck implementationProperty)
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
                    implementationCheck
                )
            )
        };

        setImplementationConstructor = Constructor(Priority.Mid, properties);

        setImplementationMethod = Method(Priority.Mid, properties);

        return Any(
            Priority.Mid,
            "2d. Has either a property as described in 2a), or a constructor or a method with a parameter that has the Implementation type and uses the field as described in 2a).",
            implementationProperty,
            setImplementationConstructor,
            setImplementationMethod
        );
    }

    /// <summary>
    /// A <see cref="ClassCheck"/> that checks the requirements of the Concrete Implementation class, which holds a
    /// <see cref="RelationCheck"/> to check the relation with the Implementation class or interface
    /// </summary>
    /// <param name="implementationCheck">The Implementation abstract <see cref="ClassCheck"/> or <see cref="InterfaceCheck"/></param>
    /// <param name="methodInImplementation">The <see cref="MethodCheck"/> in the Implementation class or interface</param>
    /// <returns>A <see cref="ClassCheck"/> that checks for a class that adheres to the requirements of an instance of
    /// the concreteImplementation</returns>
    public abstract ClassCheck ConcreteImplementationCheck(CheckBase implementationCheck, MethodCheck methodInImplementation);

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
