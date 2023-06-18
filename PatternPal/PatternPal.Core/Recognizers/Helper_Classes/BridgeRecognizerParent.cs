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
    /// <returns>A array of <see cref="ICheck"/>s containing the five class checks that a bridge pattern holds.</returns>
    public ICheck[] Checks()
    {
        // requirements checks for Implementation 
        // req. b
        MethodCheck methodInImplementation = HasMethod();

        // req. a
        CheckBase implementationCheck = HasInterfaceOrAbstractClassWithMethod(methodInImplementation);

        // requirements checks for Abstraction class
        // req. a
        ICheck fieldOrProperty = HasFieldOrProperty(out PropertyCheck implementationProperty, implementationCheck);

        // req. b
        MethodCheck methodInAbstraction = Method(Priority.Knockout);

        // req. c
        MethodCheck methodInAbstractionWithUse = HasMethodWithUse(methodInImplementation);

        // req. d
        ICheck setAvailabilityFieldOrProperty = SetAvailabilityFieldOrPropertyCheck(
            out ConstructorCheck setImplementationConstructor, out MethodCheck setImplementationMethod, 
            implementationCheck, implementationProperty);

        ClassCheck abstractionCheck = Class(Priority.High, fieldOrProperty, methodInAbstraction, methodInAbstractionWithUse, setAvailabilityFieldOrProperty);

        // requirements checks for Concrete Implementations
        // req. a & b 
        ClassCheck concreteImplementationCheck = ConcreteImplementation(implementationCheck, methodInImplementation);

        // requirements checks for Refined Abstraction
        // req. a
        ICheck inheritsFromAbstraction = Inherits(Priority.Knockout, abstractionCheck);

        // req. b
        ICheck methodInRefinedAbstraction = Method(Priority.High);

        ClassCheck refinedAbstraction = Class(Priority.High, inheritsFromAbstraction, methodInRefinedAbstraction);

        // requirements checks for Client
        // req. a 
        MethodCheck methodUseInAbstraction = Method(Priority.Mid, Uses(Priority.Mid, methodInAbstractionWithUse));

        // req. c
        ICheck setsImplementationField = SetsImplementationFieldCheck(implementationProperty, setImplementationConstructor, setImplementationMethod);

        
        ClassCheck clientCheck = Class( 
            Priority.Low,
            methodUseInAbstraction,
            Creates(Priority.Low, concreteImplementationCheck), 
            setsImplementationField );

        return new ICheck[]
        {
            implementationCheck , abstractionCheck, concreteImplementationCheck, refinedAbstraction, clientCheck
        };
        
    }

    /// <summary>
    /// A <see cref="MethodCheck"/> which will check the existence of a method.
    /// </summary>
    /// <returns>A <see cref="MethodCheck"/> depending on the type of the class it exists in. </returns>
    public abstract MethodCheck HasMethod();

    /// <summary>
    /// A <see cref="CheckBase"/> which holds either a <see cref="InterfaceCheck"/> or a <see cref="ClassCheck"/> depending
    /// on the content of the override.
    /// </summary>
    /// <param name="methodInImplementation">The method that the class or interface should have. </param>
    /// <returns>A <see cref="CheckBase"/> with a <see cref="MethodCheck"/>.</returns>
    public abstract CheckBase HasInterfaceOrAbstractClassWithMethod(MethodCheck methodInImplementation);

    /// <summary>
    /// A <see cref="ICheck"/> which checks for the existence of either a field or a property with type
    /// <paramref name="implementationCheck"/> and has either the <see cref="Modifier.Private"/> or
    /// the <see cref="Modifier.Protected"/> modifier.
    /// </summary>
    /// <param name="implementationCheck"> A <see cref="CheckBase"/> that either holds a <see cref="InterfaceCheck"/>
    /// or a <see cref="ClassCheck"/>.</param>
    /// <returns>A <see cref="ICheck"/> that checks for the existence of either a field or a property. </returns>
    public ICheck HasFieldOrProperty(out PropertyCheck implementationProperty, CheckBase implementationCheck)
    {
        implementationProperty = Property(Priority.Knockout,
            Type(Priority.Knockout, implementationCheck),
            Any(Priority.Knockout,
                Modifiers(Priority.Knockout, Modifier.Private),
                Modifiers(Priority.Knockout, Modifier.Protected))
        );

        return Any(
            Priority.Knockout,
            Field(
                Priority.Knockout,
                Type( Priority.Knockout, implementationCheck),
                Any(Priority.Knockout, 
                    Modifiers( Priority.Knockout, Modifier.Private), 
                    Modifiers(Priority.Knockout, Modifier.Protected)
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
    public MethodCheck HasMethodWithUse(MethodCheck methodInImplementation)
    {
        return Method(
            Priority.High,
            Uses(
                Priority.High,
                methodInImplementation));
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
                    Modifier.Private)),            
            Not(Priority.Mid,
                Modifiers(Priority.Mid,
                    Modifier.Protected)),
            Parameters(
                Priority.Mid,
                Type(Priority.Mid, implementationCheck))
        };

        setImplementationConstructor = Constructor(Priority.Mid, properties);

        setImplementationMethod = Method(Priority.Mid, properties);

        return Any(
            Priority.Mid,
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
    public abstract ClassCheck ConcreteImplementation(CheckBase implementationCheck, MethodCheck methodInImplementation);

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
            Uses(Priority.Low, setImplementationConstructor),
            Uses(Priority.Low, setImplementationMethod),
            Uses(Priority.Low, implementationProperty)
        );
    }

}
