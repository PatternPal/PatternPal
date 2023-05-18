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
        InterfaceCheck interfaceStrategyCheck = Interface(
            Priority.Knockout,
            interfaceMethodExecuteStrategy
        );

        // Strategy Abstract Class - option a
        ClassCheck abstractClassCheck = AbstractClass(
            Priority.Knockout,
            abstractMethodExecuteStrategy
        );

        // Strategy - a
        yield return CheckStrategy(interfaceStrategyCheck, abstractClassCheck);


        // Context Class - a
        ICheck fieldOrProperty = FieldOrPropertyContextClass(interfaceStrategyCheck, abstractClassCheck);

        // Context Class - b
        // Todo: Implement a setBy relation between method and property/field
        MethodCheck setStrategyMethod = SetStrategyMethodCheck(interfaceStrategyCheck, abstractClassCheck);

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

        // Todo: Check if methods are implemented 

        // Concrete Strategy - a
        ICheck implementationOrInherit = ImplementOrInherit(interfaceStrategyCheck, abstractClassCheck);

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

    /// <summary>
    /// Creates a <see cref="RelationCheck"/> that checks whether an entity uses a method that corresponds with the <paramref name="executeStrategyMethod"/>.
    /// </summary>
    /// <param name="executeStrategyMethod">A <see cref="MethodCheck"/> that corresponds with the executeStrategy method in the Context class </param>
    /// <returns>A <see cref="RelationCheck"/> that is used to check the usage of the executeStrategy method in the Context class</returns>
    private RelationCheck UsedExecuteStrategyMethodCheck(MethodCheck executeStrategyMethod)
    {
        return Uses(
            Priority.Low,
            executeStrategyMethod.Result
        );
    }

    /// <summary>
    /// Creates a <see cref="RelationCheck"/> that checks whether an entity uses a method that corresponds with the <paramref name="setStrategyMethod"/>.
    /// </summary>
    /// <param name="setStrategyMethod">A <see cref="MethodCheck"/> that corresponds with the setStrategy method in the Context class </param>
    /// <returns>A <see cref="RelationCheck"/> that is used to check the usage of the setStrategy method in the Context class</returns>
    private RelationCheck UsedSetStrategyMethodCheck(MethodCheck setStrategyMethod)
    {
        return Uses(
            Priority.Low,
            setStrategyMethod.Result);
    }

    /// <summary>
    /// Creates a <see cref="RelationCheck"/> that checks whether the entity creates a <paramref name="concreteClassCheck"/> object.
    /// </summary>
    /// <param name="concreteClassCheck">A <see cref="ClassCheck"/> that corresponds with the Concrete Strategy class requirements</param>
    /// <returns>A <see cref="RelationCheck"/> that is used to check if the entity creates a <paramref name="concreteClassCheck"/> object. </returns>
    private RelationCheck CreatedConcreteStrategyCheck(ClassCheck concreteClassCheck)
    {
        return Creates(
            Priority.Mid,
            concreteClassCheck.Result);
    }

    /// <summary>
    /// Creates a <see cref="RelationCheck"/> that checks whether the entity is used by the results of the <paramref name="contextClassCheck"/>.
    /// </summary>
    /// <param name="contextClassCheck"> A <see cref="ClassCheck"/> that corresponds with the Context class requirements</param>
    /// <returns>A <see cref="RelationCheck"/> that is used to check if an object of the class is stored in the <paramref name="contextClassCheck"/>. </returns>
    private RelationCheck StoredInContextClass(ClassCheck contextClassCheck)
    {
        return UsedBy(
            Priority.Mid,
            contextClassCheck.Result);
    }

    /// <summary>
    /// Creates a <see cref="RelationCheck"/> that checks whether the entity is used by the results of the <paramref name="contextClassCheck"/>.
    /// </summary>
    /// <param name="contextClassCheck"> A <see cref="ClassCheck"/> that corresponds with the Context class requirements</param>
    /// <returns>A <see cref="RelationCheck"/> that is used to check if a class is used by the <paramref name="contextClassCheck"/>. </returns>
    private RelationCheck UsageContextClass(ClassCheck contextClassCheck)
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
        );
    }

    /// <summary>
    /// Creates a <see cref="ICheck"/> that checks whether a class is an implementation of the <paramref name="strategyinterfaceStrategyCheck"/> or inherits
    /// from the <paramref name="strategyAbstractClassCheck"/>.
    /// </summary>
    /// <param name="interfaceStrategyCheck"> A <see cref="InterfaceCheck"/> that corresponds with the
    /// Strategy Interface requirements.</param>
    /// <param name="strategyAbstractClassCheck"> A <see cref="ClassCheck"/> that corresponds with the
    /// Strategy Abstract class requirements.</param>
    /// <returns>A <see cref="ICheck"/> that is used to check if a class is an implementation of the Strategy Interface or inherits from the
    /// Strategy Abstract class.</returns>
    private ICheck ImplementOrInherit(InterfaceCheck interfaceStrategyCheck, ClassCheck strategyAbstractClassCheck)
    {
        return Any(
            Priority.Knockout,
            Implements(
                Priority.Knockout,
                interfaceStrategyCheck.Result),
            Inherits(
                Priority.Knockout,
                strategyAbstractClassCheck.Result)
        );
    }

    /// <summary>
    /// Creates a <see cref="MethodCheck"/> that checks whether there is a method that either uses the
    /// executeStrategy method, entities that are results of the <paramref name="strategyInterfaceMethodCheck"/> or <paramref name="strategyAbstractMethodClassCheck"/>.
    /// </summary>
    /// <param name="strategyInterfaceMethodCheck"> A <see cref="MethodCheck"/> that checks whether there is a method in the Strategy Interface requirements.</param>
    /// <param name="strategyAbstractMethodClassCheck"> A <see cref="MethodCheck"/> that checks whether there is an abstract method in
    /// the Strategy Abstract class requirements.</param>
    /// <returns>A <see cref="MethodCheck"/> that checks the usage of this method.</returns>
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

    /// <summary>
    /// Creates a <see cref="MethodCheck"/> that checks whether there is a method that has
    /// a parameter with the type of either the <paramref name="interfaceStrategyCheck"/> or the <paramref name="strategyAbstractClassCheck"/> entities.
    /// </summary>
    /// <param name="interfaceStrategyCheck"> A <see cref="InterfaceCheck"/> that corresponds with the
    /// Strategy Interface requirements.</param>
    /// <param name="strategyAbstractClassCheck"> A <see cref="ClassCheck"/> that corresponds with the
    /// Strategy Abstract class requirements.</param>
    /// <returns>A <see cref="MethodCheck"/> that checks for the existence of a setStrategy method in the Context class.</returns>
    private MethodCheck SetStrategyMethodCheck(InterfaceCheck interfaceStrategyCheck, ClassCheck strategyAbstractClassCheck)
    {
        // Todo: Add relations between methods and fields/properties to create a more solid check
        return Method(
            Priority.High,
            Any(
                Priority.High,
                Parameters(
                    Priority.High, 
                    Type(Priority.High, interfaceStrategyCheck.Result)
                ),
                Parameters(
                    Priority.High, 
                    Type(Priority.High, strategyAbstractClassCheck.Result)
                )
            )
        );
    }

    /// <summary>
    /// Creates <see cref="CheckCollectionKind.Any"/> that checks whether there is a field or property with the type of the entities
    /// matched with the <paramref name="strategyinterfaceStrategyCheck"/> or the <paramref name="strategyAbstractClassCheck"/> type.
    /// </summary>
    /// <param name="interfaceStrategyCheck"> A <see cref="InterfaceCheck"/> that corresponds with the
    /// Strategy Interface requirements</param>
    /// <param name="strategyAbstractClassCheck"> A <see cref="ClassCheck"/> that corresponds with the
    /// Strategy Abstract class requirements</param>
    /// <returns>A <see cref="ICheck"/> that is used to check for a field or property in the Context class</returns>
    private ICheck FieldOrPropertyContextClass(InterfaceCheck interfaceStrategyCheck, ClassCheck strategyAbstractClassCheck)
    {
        return Any(
            Priority.Knockout,
            Field(
                Priority.Knockout,
                Any(
                    Priority.Knockout,
                    Type(
                        Priority.Knockout,
                        interfaceStrategyCheck.Result),
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
                        interfaceStrategyCheck.Result),
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

    /// <summary>
    /// Creates a <see cref="MethodCheck"/> that checks that there is an abstract method in a certain entity.
    /// </summary>
    /// <returns>A <see cref="MethodCheck"/> with a check for an abstract modifier</returns>
    private MethodCheck AbstractMethodExecuteStrategy()
    {
        return Method(
            Priority.High,
            Modifiers(Priority.High,
                Modifier.Abstract
            )
        );
    }

    /// <summary>
    /// Creates a <see cref="MethodCheck"/> that checks that there is a method in a certain entity.
    /// </summary>
    /// <returns>A <see cref="MethodCheck"> to denote if there is a method</see>/></returns>
    private MethodCheck InterfaceMethodExecuteStrategy()
    {
        return Method(Priority.High);
    }



    // Step 1 - create Strategy
    private ICheck CheckStrategyExistence()
    {
        return Any(
            Priority.Knockout,
            AbstractClass(Priority.Knockout),
            Interface(Priority.Knockout)
        );
    }

    // Step 2 - create executeStrategy() (abstract) method
    private ICheck CheckStrategy(InterfaceCheck interfaceStrategyCheck, ClassCheck abstractClassCheck)
    {
        return Any(
            Priority.Knockout,
            interfaceStrategyCheck,
            abstractClassCheck
        );
    }

    // Step 3 - create "Concrete Strategy" class
    private ICheck CheckConcreteStrategyExistence(ICheck implementationOrInherit)
    {
        return Class(
            Priority.Knockout,
            implementationOrInherit
        );
    }

    // Step 4 - implement / overwrite executeStrategy()
    // ToDo should be checked in implementation/inherit
    private ICheck CheckExecuteStrategy()
    {

    }


    private ICheck CheckFieldOrProperty(){}
}
