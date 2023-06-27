#region

using PatternPal.Core.Checks;
using PatternPal.Core.StepByStep;
using PatternPal.Core.StepByStep.Resources.Instructions;
using PatternPal.SyntaxTree.Models;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided files or project implement the strategy pattern.
/// </summary>
/// <remarks>
/// Requirements to fulfill the pattern:<br/>
///   Strategy:<br/>
///     a) is an interface / abstract class<br/>
///     b) has declared a method<br/>
///         b0) if the class is an abstract instead of an interface the method has to be an abstract method<br/>
///     c) is used by the Context class                                     -- checked in Context Class - c<br/>
///     d) is implemented / inherited by at least one other class           -- checked in Concrete Class - a<br/>
///     e) is implemented / inherited by at least two other classes         -- Todo if enough time<br/>
///  Concrete Strategy<br/>
///     a) is an implementation of the Strategy interface<br/>
///     b) if the class is used, it must be used via the context class      -- Todo Create stricter check<br/>
///     c) if the class is not used it should be used via the context class -- Check by Context Class<br/>
///     d) is stored in the context class   -- checked in Context Class - <br/>
///  Context<br/>
///     a) has a private field or property that has a Strategy class as type<br/>
///     b) has a function setStrategy() to set the non-public field / property with parameter of type Strategy<br/>
///     c) has a function useStrategy() to execute the strategy.<br/>
///  Client<br/>
///     a) has created an object of the type ConcreteStrategy<br/>
///     b) has used the setStrategy() in the Context class to store the ConcreteStrategy object<br/>
///     c) has executed the ConcreteStrategy via the Context class<br/>
/// </remarks>
internal class StrategyRecognizer : IRecognizer, IStepByStepRecognizer
{
    private ClassCheck ? _concreteClassCheck;
    private InterfaceCheck ? _interfaceStrategyCheck;
    private ClassCheck ? _abstractClassStrategyCheck;

    private ICheck ? _fieldStrategy;
    private ICheck ? _propertyStrategy;

    /// <inheritdoc />
    public string Name => "Strategy";

    /// <inheritdoc />
    public Recognizer RecognizerType => Recognizer.Strategy;

    /// <inheritdoc />
    IEnumerable< ICheck > IRecognizer.Create()
    {
        // Check Strategy Interface / Abstract class
        yield return CheckStrategy(
            out MethodCheck interfaceMethodExecuteStrategy,
            out MethodCheck abstractMethodExecuteStrategy);

        // Check Concrete Strategy Class
        yield return CheckConcreteStrategyExistence();

        // Check Context Class 
        yield return CheckContextClassExistence();
        yield return CheckUsageExecuteStrategy(
            out MethodCheck usageExecuteStrategy,
            out MethodCheck setStrategyMethodCheck,
            interfaceMethodExecuteStrategy,
            abstractMethodExecuteStrategy
        );

        // Client Class
        ClassCheck clientClassCheck = CheckUsageDoSomething(
            setStrategyMethodCheck,
            usageExecuteStrategy);
        yield return clientClassCheck;
    }

    /// <summary>
    /// Checks all the requirements for the the Strategy Interface / Abstract class.
    /// This method corresponds to Step 1 in the step-by-step mode : create "Strategy" interface / abstract class
    /// </summary>
    /// <param name="interfaceMethodExecuteStrategy">Is a <see cref="MethodCheck"/> that checks the
    /// existence of a method that should be present in the interface implementation of the Strategy entity</param>
    /// <param name="abstractMethodExecuteStrategy">Is a <see cref="MethodCheck"/> that checks the
    /// existence of an abstract method that should be present in the abstract class implementation
    /// of the Strategy entity</param>
    /// <returns>A <see cref="NodeCheck{TNode}"/> which checks for the existence of the Strategy entity and
    /// all its requirements.</returns>
    internal NodeCheck< INode > CheckStrategy(
        out MethodCheck interfaceMethodExecuteStrategy,
        out MethodCheck abstractMethodExecuteStrategy)
    {
        // req. b
        interfaceMethodExecuteStrategy = Method(
            Priority.Knockout,
            "1b. Has declared a method.");

        // req. a - interface option
        _interfaceStrategyCheck = Interface(
            Priority.Knockout,
            "1a. Is an interface.",
            interfaceMethodExecuteStrategy
        );

        // req. b0
        abstractMethodExecuteStrategy = Method(
            Priority.High,
            "1b. Has declared an abstract method.",
            Modifiers(
                Priority.High,
                Modifier.Abstract
            )
        );

        // req. a - abstract class option
        _abstractClassStrategyCheck = AbstractClass(
            Priority.Knockout,
            "1a. Is an abstract class.",
            abstractMethodExecuteStrategy
        );

        // Strategy implementation check
        return Any(
            Priority.Knockout,
            "1. Strategy Class",
            _interfaceStrategyCheck,
            _abstractClassStrategyCheck
        );
    }

    /// <summary>
    /// Creates a <see cref="ClassCheck"/> that checks for the existence of an entity
    /// that implements from an entity adhering to either the <see cref= "_interfaceStrategyCheck" /> check or
    /// inheriting from an entity adhering to the<see cref="_abstractClassStrategyCheck"/> abstract class.
    /// This method corresponds to Step 2 in the step-by-step mode : create "Concrete Strategy" class.
    /// </summary>
    /// <returns>A <see cref="ClassCheck"/> which checks whether a class entity inherits or implements the
    /// Strategy interface / abstract class.</returns>
    internal ClassCheck CheckConcreteStrategyExistence()
    {
        // req. a
        _concreteClassCheck = Class(
            Priority.Knockout,
            "2. Concrete Strategy Class",
            Any(
                Priority.Knockout,
                "2a. Is an implementation of the Strategy interface.",
                Implements(
                    Priority.Knockout,
                    _interfaceStrategyCheck),
                Inherits(
                    Priority.Knockout,
                    _abstractClassStrategyCheck)
            )
        );
        return _concreteClassCheck;
    }

    /// <summary>
    /// Creates a <see cref="ClassCheck"/> that checks in a class entity the existence of either a private
    /// field with the "Strategy" type or a private property with the "Strategy" type.
    /// This method corresponds to Step 3 in the step-by-step mode : create "Context" class with field or property with the Strategy type.
    /// </summary>
    /// <param name="checks"> An array of<see cref="ICheck"/>s that hold other checks that the
    /// class should adhere to.</param>
    /// <returns>A <see cref="ClassCheck"/> that checks for the existence of a private field/property and
    /// other checks that are passed to it.</returns>
    internal ClassCheck CheckContextClassExistence(
        params ICheck[ ] checks)
    {
        // req. a
        _fieldStrategy = Field(
            Priority.Knockout,
            "3a. Has a private field or property that has a Strategy class as type.",
            Any(
                Priority.Knockout,
                Type(
                    Priority.Knockout,
                    _interfaceStrategyCheck),
                Type(
                    Priority.Knockout,
                    _abstractClassStrategyCheck)
            ),
            Modifiers(
                Priority.Knockout,
                Modifier.Private
            )
        );
        _propertyStrategy = Property(
            Priority.Knockout,
            "3a. Has a private field or property that has a Strategy class as type.",
            Any(
                Priority.Knockout,
                Type(
                    Priority.Knockout,
                    _interfaceStrategyCheck),
                Type(
                    Priority.Knockout,
                    _abstractClassStrategyCheck)
            ),
            Modifiers(
                Priority.Knockout,
                Modifier.Private
            )
        );

        return Class(
            Priority.Knockout,
            "3. Context Class",
            checks.Append(
                Any(
                    Priority.Knockout,
                    _fieldStrategy,
                    _propertyStrategy)
            ).ToArray());
    }

    /// <summary>
    /// Creates a <see cref="MethodCheck"/> that checks whether a method uses the <see cref="_fieldOrPropertyStrategy"/>
    /// it returns this check in combination with the <see cref="ICheck"/>s in <see cref="CheckContextClassExistence"/>.
    /// This method corresponds to Step 4 in the step-by-step mode : create function "setStrategy()".
    /// </summary>
    /// <param name="setStrategyMethodCheck">A <see cref="MethodCheck"/> that checks for the uses relation with
    /// the <see cref="_fieldOrPropertyStrategy"/>.</param>
    /// <param name="checks">Other <see cref="ICheck"/>s that should be checked in combination with the <paramref name="setStrategyMethodCheck"/>.</param>
    /// <returns>A <see cref="ClassCheck"/> that combines the <paramref name="setStrategyMethodCheck"/> in combination with
    /// <see cref="CheckContextClassExistence"/> <see cref="ClassCheck"/>.</returns>
    internal ClassCheck CheckSetStrategy(
        out MethodCheck setStrategyMethodCheck,
        params ICheck[ ] checks)
    {
        // req. b
        setStrategyMethodCheck = Method(
            Priority.High,
            "3b. Has a function setStrategy() to set the non-public field / property with parameter of type Strategy.",
            Any(
                Priority.High,
                Uses(
                    Priority.High,
                    _fieldStrategy),
                Uses(
                    Priority.High,
                    _propertyStrategy)
            ),
            Any(
                Priority.High,
                Parameters(
                    Priority.High,
                    Type(
                        Priority.High,
                        _interfaceStrategyCheck
                    )
                ),
                Parameters(
                    Priority.High,
                    Type(
                        Priority.High,
                        _abstractClassStrategyCheck
                    )
                )
            )
        );

        return CheckContextClassExistence(
            new ICheck[ ]
            {
                setStrategyMethodCheck
            }.Concat(checks).ToArray());
    }

    /// <summary>
    /// Creates a <see cref="MethodCheck"/> that checks whether a Method uses either the <paramref name="interfaceMethodExecuteStrategy"/>
    /// or <paramref name="abstractMethodExecuteStrategy"/>, it returns this in combination with the checks in <see cref="CheckSetStrategy"/>.
    /// This method corresponds to Step 5 in the step-by-step mode : create function "doSomething()" which uses "executeStrategy()".
    /// </summary>
    /// <param name="usageExecuteStrategy">A <see cref="MethodCheck"/> that checks whether a Method uses
    /// either the <paramref name="interfaceMethodExecuteStrategy"/> or <paramref name="abstractMethodExecuteStrategy"/>.</param>
    /// <param name="setStrategyMethodCheck">A <see cref="MethodCheck"/> that is created in <see cref="CheckSetStrategy"/>.</param>
    /// <param name="interfaceMethodExecuteStrategy">A <see cref="MethodCheck"/> that is created in <see cref="CheckStrategy"/>.</param>
    /// <param name="abstractMethodExecuteStrategy">A <see cref="MethodCheck"/> that is created in <see cref="CheckStrategy"/>.</param>
    /// <returns></returns>
    internal ClassCheck CheckUsageExecuteStrategy(
        out MethodCheck usageExecuteStrategy,
        out MethodCheck setStrategyMethodCheck,
        MethodCheck interfaceMethodExecuteStrategy,
        MethodCheck abstractMethodExecuteStrategy
    )
    {
        // req. c
        usageExecuteStrategy = Method(
            Priority.Mid,
            "3c. Has a function useStrategy() to execute the strategy.",
            Any(
                Priority.Mid,
                Uses(
                    Priority.Mid,
                    interfaceMethodExecuteStrategy
                ),
                Uses(
                    Priority.Mid,
                    abstractMethodExecuteStrategy
                )
            )
        );

        return CheckSetStrategy(
            out setStrategyMethodCheck,
            usageExecuteStrategy);
    }

    /// <summary>
    /// Creates a <see cref="RelationCheck"/> that verifies whether the current entity creates an instance of <see cref="_concreteClassCheck"/>.
    /// It returns this relation check along with the incoming <paramref name="checks"/> in a <see cref="ClassCheck"/>.
    /// This method corresponds to Step 6 in the step-by-step mode: create a class "Client" which creates a "Concrete Strategy" object.
    /// </summary>
    /// <param name="checks">A list of <see cref="ICheck"/> instances included in the resulting <see cref="ClassCheck"/>.</param>
    /// <returns>A <see cref="ClassCheck"/> containing the relation check and the specified <paramref name="checks"/>.</returns>
    internal ClassCheck CheckCreationConcreteStrategy(
        params ICheck[ ] checks)
    {
        // req. a
        RelationCheck creationConcreteStrategy = Creates(
            Priority.Mid,
            "4a. Has created an object of the type ConcreteStrategy.",
            _concreteClassCheck);

        return Class(
            Priority.Mid,
            "4. Client class",
            new ICheck[ ]
            {
                creationConcreteStrategy
            }.Concat(checks).ToArray()
        );
    }

    /// <summary>
    /// Creates a <see cref="RelationCheck"/> that verifies whether the current entity uses the <paramref name="setStrategyMethodCheck"/> method.
    /// It returns this relation check along with the <paramref name="checks"/> in the <see cref="ClassCheck"/> created by the <see cref="CheckCreationConcreteStrategy"/> method.
    /// This method corresponds to Step 7 in the step-by-step mode: use "setStrategy()".
    /// </summary>
    /// <param name="setStrategyMethodCheck">A <see cref="MethodCheck"/> instance created in the <see cref="CheckSetStrategy"/> method.</param>
    /// <param name="checks">A list of <see cref="ICheck"/> instances to be checked in combination with the <see cref="RelationCheck"/>.</param>
    /// <returns>A <see cref="ClassCheck"/> containing the relation check and the specified <paramref name="checks"/>.</returns>
    internal ClassCheck CheckUsageSetStrategy(
        MethodCheck setStrategyMethodCheck,
        params ICheck[ ] checks)
    {
        // req. b
        RelationCheck usageSetStrategy = Uses(
            Priority.Low,
            "4b. Has used the setStrategy() in the Context class to store the ConcreteStrategy object.",
            setStrategyMethodCheck);

        return CheckCreationConcreteStrategy(
            new ICheck[ ]
            {
                usageSetStrategy
            }.Concat(checks).ToArray()
        );
    }

    /// <summary>
    /// Creates a <see cref="RelationCheck"/> that verifies the usage <paramref name="usageExecuteStrategy"/>, this is then added to the
    /// list if <see cref="ICheck"/>s in the <see cref="ClassCheck"/> created in <see cref="CheckUsageSetStrategy"/>.
    /// This method corresponds to Step 8 in the step-by-step mode: use "doSomething()".
    /// </summary>
    /// <param name="setStrategyMethodCheck">A <see cref="MethodCheck"/> instance created in the <see cref="CheckSetStrategy"/> method.</param>
    /// <param name="usageExecuteStrategy">A <see cref="MethodCheck"/> instance representing the usage of the execute strategy.</param>
    /// <returns>A <see cref="ClassCheck"/> containing the check for the usage of the method adhering with the <paramref name="usageExecuteStrategy"/> check
    /// and the Checks in the <see cref="CheckUsageSetStrategy"/>.</returns>
    internal ClassCheck CheckUsageDoSomething(
        MethodCheck setStrategyMethodCheck,
        MethodCheck usageExecuteStrategy
    )
    {
        // req. c
        RelationCheck usageDoSomething = Uses(
            Priority.Low,
            "4c. Has executed the ConcreteStrategy via the Context class.",
            usageExecuteStrategy
        );
        return CheckUsageSetStrategy(
            setStrategyMethodCheck,
            usageDoSomething);
    }

    public List<IInstruction> GenerateStepsList()
    {
        List<IInstruction> generateStepsList = new();

        MethodCheck strategyMethod = Method(Priority.Knockout);
        InterfaceCheck strategy = 
            Interface(
                Priority.Knockout,
                strategyMethod
            );

        generateStepsList.Add(
            new SimpleInstruction(
               "TODO",
               "TODO",
                new List<ICheck> { strategy }));

        ClassCheck concreteStrategy = 
            Class(
                Priority.Knockout,
                Implements(
                    Priority.Knockout, 
                    strategy
                )
            );
        _concreteClassCheck = concreteStrategy;

        generateStepsList.Add(
            new SimpleInstruction(
                "TODO",
                "TODO",
                new List<ICheck>
                {
                    strategy,
                    concreteStrategy
                }));

        FieldCheck strategyField = 
            Field(
                Priority.Knockout,
                Type(
                    Priority.Knockout,
                    strategy
                ),
                Modifiers(
                    Priority.Knockout,
                    Modifier.Private
                )
            );
        MethodCheck usageStrategyMethod =
            Method(
                Priority.Knockout,
                Uses(
                    Priority.High,
                    strategyMethod
                )
            );
        MethodCheck setStrategyMethod =
            Method(
                Priority.Knockout,
                Uses(
                    Priority.Knockout,
                    strategyField
                ),
                Parameters(
                    Priority.Knockout,
                    Type(
                        Priority.Knockout,
                        strategy
                    )
                )
            );
        ClassCheck context =
            Class(
                Priority.Knockout,
                strategyField,
                usageStrategyMethod,
                setStrategyMethod
            );

        generateStepsList.Add(
            new SimpleInstruction(
                "TODO",
                "TODO",
                new List<ICheck>
                {
                    strategy,
                    concreteStrategy,
                    context
                }));

        ClassCheck client = CheckUsageDoSomething(
            setStrategyMethod,
            usageStrategyMethod);

        generateStepsList.Add(
            new SimpleInstruction(
                "TODO",
                "TODO",
                new List<ICheck>
                {
                    strategy,
                    concreteStrategy,
                    context,
                    client
                }));

        return generateStepsList;
    }
}
