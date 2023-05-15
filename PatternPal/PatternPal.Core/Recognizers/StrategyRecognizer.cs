#region

using PatternPal.Core.Checks;
using PatternPal.SyntaxTree.Models;
using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;


/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided file is an implementation
/// of the strategy pattern.
/// </summary>
/// <remarks>
/// Requirements to fulfill the pattern:<br/>
///   Strategy:
///     a) is an interface / abstract class
///     b) has declared a method
///         b0) if the class is an abstract instead of an interface the method has to be an abstract method
///     c) is used by the Context class                                     -- checked in Context Class - c
///     d) is implemented / inherited by at least one other class           -- checked in Concrete Class - a
///     e) is implemented / inherited by at least two other classes
///  Context
///     a) has a private field or property that has a Strategy class as type
///     b) has a function setStrategy() to set the non-public field / property with parameter of type Strategy
///     c) as a function useStrategy() to execute the strategy.
///  Concrete Strategy
///     a) is an implementation of the Strategy interface
///     b) if the class is used, it must be used via the context class
///     c) if the class is not used it should be used via the context class
///     d) is stored in the context class
///  Client
///     a) has created an object of the type ConcreteStrategy
///     b) has used the setStrategy() in the Context class to store the ConcreteStrategy object
///     c) has executed the ConcreteStrategy via the Context class
/// </remarks>

internal class StrategyRecognizer : IRecognizer
{
    /// <summary>
    /// A method which creates a lot of <see cref="ICheck"/>s that each adheres to the requirements a strategy pattern needs to have implemented.
    /// It returns the requirements in a tree structure stated per class.
    /// </summary>
    internal IEnumerable<ICheck> Create()
    {
        // Strategy Interface - b
        MethodCheck interfaceMethodExecuteStrategy = InterfaceMethodExecuteStrategy();

        // Strategy Abstract Class - b0
        MethodCheck abstractMethodExecuteStrategy = AbstractMethodExecuteStrategy();
        
        // Todo: Strategy - e

        // Strategy Interface - option a
        InterfaceCheck interfaceCheck = Interface(
            Priority.Knockout,
            interfaceMethodExecuteStrategy
        );

        // Strategy Abstract Class - option a
        ClassCheck abstractClassCheck = AbstractClass(
            Priority.Knockout,
            abstractMethodExecuteStrategy
        );

        // Strategy - a
        yield return Any(
            Priority.Knockout,
            interfaceCheck,
            abstractClassCheck
        );


        // Context Class - a
        ICheck fieldOrProperty = FieldOrPropertyContextClass(interfaceCheck, abstractClassCheck);

        // Context Class - b
        // Todo: Implement a setBy relation between method and property/field
        MethodCheck setStrategyMethod = SetStrategyMethodCheck(interfaceCheck, abstractClassCheck);

        // Context Class - c
        MethodCheck executeStrategyMethod = ExecuteStrategyMethodCheck(interfaceMethodExecuteStrategy, abstractMethodExecuteStrategy);

        // Context Class 
        ClassCheck contextClassCheck = Class(
            Priority.Knockout,
            fieldOrProperty,
            setStrategyMethod,
            executeStrategyMethod
        );
        yield return contextClassCheck;


        // Concrete Strategy - a
        ICheck implementationOrInherit = ImplementOrInherit(interfaceCheck, abstractClassCheck);

        // Concrete Strategy - b
        // Todo: Ask Matteo how to check
        ICheck usageContextClassCheck = UsageContextClass(contextClassCheck);

        // Concrete Strategy - c
        // Todo: Ask Matteo how to check
        // Todo: Do we also want to check if used by setStrategyMethod? And executeStrategyMethod?
        ICheck storedInContextClassCheck = StoredInContextClass(contextClassCheck);

        // Concrete Strategy Class
        ClassCheck concreteClassCheck = Class(
            Priority.Knockout,
            implementationOrInherit,
            usageContextClassCheck,
            storedInContextClassCheck
        );
        yield return concreteClassCheck;

        
        // Client class - a
        ICheck createdConcreteStrategyCheck = CreatedConcreteStrategyCheck(concreteClassCheck);

        // Client class - b
        ICheck usedSetStrategyMethodCheck = UsedSetStrategyMethodCheck(setStrategyMethod);

        // Client class - c
        ICheck usedExecuteStrategyMethodCheck = UsedExecuteStrategyMethodCheck(executeStrategyMethod);


        // Client Class
        ClassCheck clientClassCheck = Class(
            Priority.Mid,
            createdConcreteStrategyCheck,
            usedSetStrategyMethodCheck,
            usedExecuteStrategyMethodCheck
        );
        yield return clientClassCheck;

    }

    private ICheck UsedExecuteStrategyMethodCheck(MethodCheck executeStrategyMethod)
    {
        return Uses(
            Priority.Low,
            executeStrategyMethod.Result
        );
    }

    private ICheck UsedSetStrategyMethodCheck(MethodCheck setStrategyMethod)
    {
        return Uses(
            Priority.Low,
            setStrategyMethod.Result)
    }

    private ICheck CreatedConcreteStrategyCheck(ClassCheck concreteClassCheck)
    {
        return Creates(
            Priority.Mid,
            concreteClassCheck.Result);
    }

    private ICheck StoredInContextClass(ClassCheck contextClassCheck)
    {
        return UsedBy(
            Priority.Mid,
            contextClassCheck.Result);
    }

    private ICheck UsageContextClass(ClassCheck contextClassCheck)
    {
        // combination of 5b + bc
        return Any(
            Priority.High,
            UsedBy(
                Priority.High,
                contextClassCheck.Result
            ),
            UsedBy(
                Priority.Mid,
                contextClassCheck.Result
            )
        )
    }

    private ICheck ImplementOrInherit(InterfaceCheck strategyInterfaceCheck, ClassCheck strategyAbstractClassCheck)
    {
        return Any(
            Priority.Knockout,
            Implements(
                Priority.Knockout,
                strategyInterfaceCheck.Result),
            Inherits(
                Priority.Knockout,
                strategyAbstractClassCheck.Result)
        );
    }

    private MethodCheck ExecuteStrategyMethodCheck(MethodCheck strategyInterfaceMethodCheck, MethodCheck strategyAbstractMethodClassCheck)
    {
        // Todo: Don't think this is complete 
        return Method(
            Priority.Mid,
            Any(
                Priority.Mid,
                Uses(
                    Priority.Mid,
                    strategyInterfaceMethodCheck.Result
                ),
                Uses(
                    Priority.Mid,
                    strategyAbstractMethodClassCheck.Result
                )
            )
        );
    }

    private MethodCheck SetStrategyMethodCheck(InterfaceCheck strategyInterfaceCheck, ClassCheck strategyAbstractClassCheck)
    {
        // Todo: Add relations between methods and fields/properties 
        return Method(
            Priority.High,
            Any(
                Priority.High,
                Parameters(
                    Priority.High, 
                    Type(Priority.High, strategyInterfaceCheck.Result)
                ),
                Parameters(
                    Priority.High, 
                    Type(Priority.High, strategyAbstractClassCheck.Result)
                )
            )
        );
    }

    private ICheck FieldOrPropertyContextClass(InterfaceCheck strategyInterfaceCheck, ClassCheck strategyAbstractClassCheck)
    {
        return Any(
            Priority.Knockout,
            Field(
                Priority.Knockout,
                Any(
                    Priority.Knockout,
                    Type(
                        Priority.Knockout,
                        strategyInterfaceCheck.Result),
                    Type(
                        Priority.Knockout,
                        strategyAbstractClassCheck.Result)
                ),
                Modifiers(
                    Priority.Knockout,
                    Modifier.Private
                )
            ),
            Property(
                Priority.Knockout,
                Any(
                    Priority.Knockout,
                    Type(
                        Priority.Knockout,
                        strategyInterfaceCheck.Result),
                    Type(
                        Priority.Knockout,
                        strategyAbstractClassCheck.Result)
                ),
                Modifiers(
                    Priority.Knockout,
                    Modifier.Private
                )
            )
        );
    }

    private MethodCheck AbstractMethodExecuteStrategy()
    {
        return Method(
            Priority.High,
            Modifiers(Priority.High,
                Modifier.Abstract
            )
        );
    }

    private MethodCheck InterfaceMethodExecuteStrategy()
    {
        return Method(Priority.High);
    }
}
