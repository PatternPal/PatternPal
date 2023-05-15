#region

using PatternPal.Core.Checks;
using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;

internal class StrategyRecognizer : IRecognizer
{    
    /* Pattern:              Strategy
     * Original code source: https://github.com/exceptionnotfound/DesignPatterns/blob/master/Strategy/CookMethod.cs
     * 
     * 
     * Requirements to fullfill the pattern:
     *         Strategy interface
     *               a) is an interface	/ abstract class
     *               b) has declared a method
     *                     1) if the class is an abstract instead of an interface the method has to be an abstract method
     *               c) is used by another class
     *               d) is implemented / inherited by at least one other class
     *               e) is implemented / inherited by at least two other classes
     *         Concrete strategy
     *               a) is an implementation of the Strategy interface
     *               b) if the class is used, it must be used via the context class
     *               c) if the class is not used it should be used via the context class
     *               d) is stored in the context class
     *         Context
     *               a) has a private field or property that has a Strategy class as type 
     *               b) has a function setStrategy() to set the non-public field / property with parameter of type Strategy
     *               c) has a function useStrategy() to execute the strategy. 
     *         Client
     *               a) has created an object of the type ConcreteStrategy
     *               b) has used the setStrategy() in the Context class to store the ConcreteStrategy object
     *               c) has executed the ConcreteStrategy via the Context class
     */
    internal IEnumerable<ICheck> Create()
    {
        // Concrete Strategy Class
        ClassCheck concreteClassCheck = Class(
            Priority.Knockout,
            Any(
                Priority.Knockout,
                Implements(
                    Priority.Knockout,
                    interfaceCheck.Result),
                Inherits(
                    Priority.Knockout,
                    abstractClassCheck.Result)
            ),
            UsedBy(
                Priority.Mid,
                contextClassCheck.MatchedEntities
            )
        );

        // Context Class 
        ClassCheck contextClassCheck = Class(
            Priority.Knockout,
            FieldOrPropertyContextClass(),
            SetStrategyMethodCheck(),
            UseStrategyMethodCheck()
        );


        // Client Class
        ClassCheck clientClassCheck = Class(
            Priority.Mid,
            Create(

                )
            )


        // Option 1 for the Strategy Interface
        InterfaceCheck interfaceCheck = Interface(
            Priority.Knockout,
            InterfaceMethodExecuteStrategy(),
            UsedBy(
                Priority.Mid,
                contextClassCheck.Result),
            ImplementedBy(
                Priority.Knockout,
                concreteClassCheck.Result
            )
        );

        // Option 2 for the Strategy Interface
        ClassCheck abstractClassCheck = AbstractClass(
            Priority.Knockout,
            AbstractMethodExecuteStrategy(),
            UsedBy(
                Priority.Mid,
                contextClassCheck.Result),
            InheritedBy(
                Priority.Knockout,
                concreteClassCheck.Result
            )
        );

        // Strategy Interface
        yield return Any(
            Priority.Knockout,
            interfaceCheck,
            abstractClassCheck
        );
    }

    private MethodCheck UseStrategyMethodCheck()
    {
        return Method(
            Priority.Mid,
            Any(
                Priority.Mid,
                Uses(
                    Priority.Mid,
                    interfaceCheck.Result
                ),
                Uses(
                    Priority.Mid,
                    concreteClassCheck.Result)
            )
        );
    }

    private MethodCheck SetStrategyMethodCheck()
    {
        return Method(
            Priority.High,
            Uses(
                Priority.High,
                FieldOrProperty)
            // If Uses is not an option due to its relation with a property or field we could check on param?
        );
    }

    private CheckCollection FieldOrPropertyContextClass()
    {
        return Any(
            Priority.Knockout,
            Field(
                Priority.Knockout,
                Any(
                    Priority.Knockout,
                    Type(
                        Priority.Knockout,
                        interfaceCheck.Result),
                    Type(
                        Priority.Knockout,
                        abstractClassCheck.Result)
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
                        interfaceCheck.Result),
                    Type(
                        Priority.Knockout,
                        abstractClassCheck.Result)
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
