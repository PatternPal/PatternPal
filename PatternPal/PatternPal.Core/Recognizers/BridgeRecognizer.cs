#region 

using PatternPal.Core.Recognizers.Helper_Classes;
using PatternPal.Core.StepByStep;
using PatternPal.Core.StepByStep.Resources.Instructions;
using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided files or project implement the bridge pattern
/// </summary>
/// <remarks>
/// Requirements for the Implementation interface or abstract class:<br/>
///     a) is an interface or abstract class <br/>
///     b) has at least one (if possible: abstract) method <br/>
/// <br/>
///
/// Requirements for the Abstraction class: <br/>
///     a) has a private/protected field or property with the type of the Implementation interface or abstract class <br/>
///     b) has a method <br/>
///     c) has a method that calls a method in the Implementation interface or abstract class <br/>
///     d) has either <br/>
///         1. the property option as described in a) <br/>
///         2. a constructor with a parameter with the Implementation type and that uses the field as described in a) <br/>
///         3. a method with a parameter with the Implementation type and that uses the field as described in a)  <br/>
/// <br/>
/// 
/// Requirements for the Concrete Implementations: <br/>
///     a) is an implementation of the Implementation interface or inherits from the 'Implementation' abstract class <br/>
///     b) if Implementation is an abstract class it should override it's abstract methods <br/>
/// <br/>
/// 
/// Requirements for the Refined Abstraction: <br/>
///     a) inherits from the Abstraction class <br/>
///     b) has an method <br/>
/// <br/>
///
/// Requirements for the Client class: <br/>
///     a) calls a method in the Abstraction class <br/>
///     b) creates a Concrete Implementation instance <br/>
///     c) sets the field or property in Abstraction, either through <br/>
///         1. it is a property and it sets this <br/>
///         2. a constructor as described in Abstraction d2 <br/>
///         3. a method as described in Abstraction d3 <br/>
/// </remarks>
internal class BridgeRecognizer : IRecognizer, IStepByStepRecognizer
{
    /// <inheritdoc cref="IRecognizer" />
    public string Name => "Bridge";

    /// <inheritdoc cref="IRecognizer"/>
    public Recognizer RecognizerType => Recognizer.Bridge;

    /// <inheritdoc />
    public List<IInstruction> GenerateStepsList()
    {
        return new List <IInstruction>() 
        {
            // Step 1. Create the Implementation Interface or Abstract Class + method
            new SimpleInstruction(
                BridgeInstructions.Step1,
                BridgeInstructions.Explanation1,
                new List<ICheck>
                {
                    _abstractBridge.ImplementationClass
                }
            ),
            // Step 2. Create the Abstraction class with private / protected field or property
            new SimpleInstruction(
                BridgeInstructions.Step2,
                BridgeInstructions.Explanation2,
                new List<ICheck>
                {
                    _abstractBridge.ImplementationClass,
                    Class(
                        Priority.Knockout,
                        _abstractBridge.AbstractionFieldOrPropertyCheck
                    )
                }
            ),
            // Step 3. Create a method class in the Abstraction class that calls the method in Implementation
            new SimpleInstruction(
                BridgeInstructions.Step3,
                BridgeInstructions.Explanation3,
                new List<ICheck>
                {
                    Any(
                        Priority.Knockout,
                        _abstractBridge.ImplementationClass,
                        _interfaceBridge.ImplementationClass
                    ),
                    Any(
                        Priority.Knockout,
                        Class(
                            Priority.Knockout,
                            _abstractBridge.AbstractionFieldOrPropertyCheck,
                            _abstractBridge.AbstractionMethodUsesImplementationMethod
                        ),
                        Class(
                            Priority.Knockout,
                            _interfaceBridge.AbstractionFieldOrPropertyCheck,
                            _interfaceBridge.AbstractionMethodUsesImplementationMethod
                        )
                    )
                }
            ),
            // Step 4. Create an instance that can set the value of the field if chosen in Step 2
            new SimpleInstruction(
                BridgeInstructions.Step4,
                BridgeInstructions.Explanation4,
                new List<ICheck>
                {
                    Any(
                        Priority.Knockout,
                        _abstractBridge.ImplementationClass,
                        _interfaceBridge.ImplementationClass
                    ),
                    Any( 
                        Priority.Knockout,
                        _abstractBridge.AbstractionClass,  
                        _interfaceBridge.AbstractionClass
                    )
                }
            ),
            // Step 5. Create a class that inherits form the Implementation abstract class or inherits from the
            // Implementation interface. In the first case it should override the method. 
            new SimpleInstruction(
                BridgeInstructions.Step5,
                BridgeInstructions.Explanation5,
                new List<ICheck>
                {
                    Any(
                        Priority.Knockout,
                        _abstractBridge.ImplementationClass,
                        _interfaceBridge.ImplementationClass
                    ),
                    Any(
                        Priority.Knockout,
                        _abstractBridge.AbstractionClass,
                        _interfaceBridge.AbstractionClass
                    ),
                    Any(
                        Priority.Knockout,
                        _abstractBridge.ConcreteImplementationClass,
                        _interfaceBridge.ConcreteImplementationClass
                    )
                }
            ),
            // Step 6. Create a Refined Abstraction class that inherits from the Abstraction class and has a method
            new SimpleInstruction(
                BridgeInstructions.Step6,
                BridgeInstructions.Explanation6,
                new List<ICheck>
                {
                    Any(
                        Priority.Knockout,
                        _abstractBridge.ImplementationClass,
                        _interfaceBridge.ImplementationClass
                    ),
                    Any(
                        Priority.Knockout,
                        _abstractBridge.AbstractionClass,
                        _interfaceBridge.AbstractionClass
                    ),
                    Any(
                        Priority.Knockout,
                        _abstractBridge.ConcreteImplementationClass,
                        _interfaceBridge.ConcreteImplementationClass
                    ),
                    Any( 
                        Priority.Knockout,
                        _abstractBridge.RefinedAbstractionClassCheck(_abstractBridge.AbstractionClass),
                        _interfaceBridge.RefinedAbstractionClassCheck(_interfaceBridge.AbstractionClass)
                    )
                }
            ),
            // Step 7. Create a Client class that uses a method in the Abstraction class
            new SimpleInstruction(
                BridgeInstructions.Step7,
                BridgeInstructions.Explanation7,
                new List<ICheck>
                {
                    Any(
                        Priority.Knockout,
                        _abstractBridge.ImplementationClass,
                        _interfaceBridge.ImplementationClass
                    ),
                    Any(
                        Priority.Knockout,
                        _abstractBridge.AbstractionClass,
                        _interfaceBridge.AbstractionClass
                    ),
                    Any(
                        Priority.Knockout,
                        _abstractBridge.ConcreteImplementationClass,
                        _interfaceBridge.ConcreteImplementationClass
                    ),
                    Any(
                        Priority.Knockout,
                        _abstractBridge.RefinedAbstractionClassCheck(_abstractBridge.AbstractionClass),
                        _interfaceBridge.RefinedAbstractionClassCheck(_interfaceBridge.AbstractionClass)
                    ),
                    Any(
                        Priority.Knockout,
                        Class(Priority.Knockout, _abstractBridge.ClientUsesMethod),
                        Class(Priority.Knockout, _interfaceBridge.ClientUsesMethod)
                    )
                }
            ),
            // Step 8. Let the Client class create a Concrete Implementation instance and pass it through either a 
            // property, constructor or method to the Abstraction class
            new SimpleInstruction(
                BridgeInstructions.Step8,
                BridgeInstructions.Explanation8,
                new List<ICheck>
                {
                    Any(
                        Priority.Knockout,
                        _abstractBridge.ImplementationClass,
                        _interfaceBridge.ImplementationClass
                    ),
                    Any(
                        Priority.Knockout,
                        _abstractBridge.AbstractionClass,
                        _interfaceBridge.AbstractionClass
                    ),
                    Any(
                        Priority.Knockout,
                        _abstractBridge.ConcreteImplementationClass,
                        _interfaceBridge.ConcreteImplementationClass
                    ),
                    Any(
                        Priority.Knockout,
                        _abstractBridge.RefinedAbstractionClassCheck(_abstractBridge.AbstractionClass),
                        _interfaceBridge.RefinedAbstractionClassCheck(_interfaceBridge.AbstractionClass)
                    ),
                    Any(
                        Priority.Knockout,
                        _abstractBridge.ClientClass,
                        _interfaceBridge.ClientClass
                    )
                }
            ),
        };
    }
    

    readonly BridgeRecognizerParent _abstractBridge = new BridgeRecognizerAbstractClass();
    readonly BridgeRecognizerParent _interfaceBridge = new BridgeRecognizerInterface();

    /// <inheritdoc />
    public IEnumerable<ICheck> Create()
    {
        yield return Any(
            Priority.Knockout,
            Any(Priority.Knockout, _abstractBridge.Checks()),
            All(Priority.Knockout, _interfaceBridge.Checks()));
    }
}
