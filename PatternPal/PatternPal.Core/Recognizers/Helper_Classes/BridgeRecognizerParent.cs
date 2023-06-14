#region
using PatternPal.SyntaxTree.Models;
using static PatternPal.Core.Checks.CheckBuilder;
#endregion

namespace PatternPal.Core.Recognizers.Helper_Classes;

internal abstract class BridgeRecognizerParent
{
    public ICheck[] Checks()
    {
        // requirements checks for Implementation 
        // req. b
        MethodCheck methodInImplementation = HasMethod();

        // req. a
        CheckBase implementationCheck = HasInterfaceOrAbstractClassWithMethod(methodInImplementation);

        // requirements checks for Abstraction class
        // req. a
        ICheck fieldOrProperty = HasFieldOrProperty(implementationCheck);

        // req. b
        MethodCheck methodInAbstraction = Method(Priority.Knockout);

        // req. c
        MethodCheck methodInAbstractionWithUse = HasMethodWithUse(methodInImplementation);

        ClassCheck abstractionCheck = Class(Priority.High, fieldOrProperty, methodInAbstraction, methodInAbstractionWithUse);

        // requirements checks for Concrete Implementations
        // req. a & b 
        ClassCheck concreteImplementationCheck = ConcreteImplementation(implementationCheck, methodInImplementation);

        // requirements checks for Refined Abstraction
        // req. a
        ICheck inheritsFromAbstraction = Inherits(Priority.Low, abstractionCheck);

        // req. b
        ICheck methodInRefinedAbstraction = Method(Priority.Low);

        ClassCheck refinedAbstraction = Class(Priority.Low, inheritsFromAbstraction, methodInRefinedAbstraction);

        // requirements checks for Client
        // req. a 
        MethodCheck methodUseInAbstraction = Method(Priority.Mid, Uses(Priority.Mid, methodInAbstractionWithUse));

        // req. b & c
        ClassCheck clientCheck = Class( 
            Priority.Low,
            Uses(Priority.Mid, methodInAbstractionWithUse),
            Creates(Priority.Low, concreteImplementationCheck), 
            Uses(Priority.Low, fieldOrProperty) );

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
    public ICheck HasFieldOrProperty(CheckBase implementationCheck)
    {
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
            Property(Priority.Knockout, 
                Type(Priority.Knockout, implementationCheck), 
                Any(Priority.Knockout, 
                    Modifiers(Priority.Knockout, Modifier.Private), 
                    Modifiers(Priority.Knockout, Modifier.Protected))
            )
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
    /// A <see cref="ClassCheck"/> that checks the requirements of the Concrete Implementation class, which holds a
    /// <see cref="RelationCheck"/> to check the relation with the Implementation class or interface
    /// </summary>
    /// <param name="implementationCheck">The Implementation abstract <see cref="ClassCheck"/> or <see cref="InterfaceCheck"/></param>
    /// <param name="methodInImplementation">The <see cref="MethodCheck"/> in the Implementation class or interface</param>
    /// <returns>A <see cref="ClassCheck"/> that checks for a class that adheres to the requirements of an instance of
    /// the concreteImplementation</returns>
    public abstract ClassCheck ConcreteImplementation(CheckBase implementationCheck, MethodCheck methodInImplementation);
}
