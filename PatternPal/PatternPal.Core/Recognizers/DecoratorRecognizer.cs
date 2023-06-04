#region

using PatternPal.Core.StepByStep;
using PatternPal.SyntaxTree.Models;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided files or project implements the decorator pattern
/// </summary>
/// <remarks>
/// Requirements to fulfill the pattern:<br/>
/// 1) Requirements for Component
/// a) is an interface / abstract class
/// b) has declared a method
///     i) if the class is an abstract instead of an interface the method has to be an abstract method
/// c) is implemented / inherited by at least two other classes<br/>
/// 2) Requirements for Concrete Component
/// a) is an implementation of the Component interface<br/>
/// 3) Requirements for Base Decorator
/// a) is an abstract class
/// b) has a field of type Component
/// c) has a constructor with a parameter of type Component, which it passes to its field
/// d) calls the method of its field in the implementation of the method of Component
/// e) is an implementation of the Component interface<br/>
/// 4) Requirements for Concrete Decorator
/// a) inherits from Base Decorator
/// b) calls the method of its parent in the implementation of the method of Component
/// c) has a function providing extra behaviour which it calls in the implementation of the method of Component<br/>
/// TODO could also be a requirement to have an abstract method in Base Decorator, but this might be enough as both are oke and putting this in an Any is no doing
/// 5) Requirements for Client
/// a) has created an object of the type ConcreteComponent
/// b) has created an object of the type ConcreteDecorator, to which it passes the ConcreteComponent
/// c) has called the method of ConcreteDecorator
/// </remarks>
internal class DecoratorRecognizer : IRecognizer
{
    /// <inheritdoc />
    public string Name => "Decorator";

    /// <inheritdoc />
    public Recognizer RecognizerType => Recognizer.Decorator;

    /// <summary>
    /// A method which creates a lot of <see cref="ICheck"/>s that each adheres to the requirements a decorator pattern needs to have implemented.
    /// It returns the requirements in a tree structure stated per class.
    /// </summary>
    public IEnumerable<ICheck> Create()
    {
        MethodCheck componentMethod = Method(Priority.Knockout);

        //Could also be an abstract class TODO how
        InterfaceCheck component =
            Interface(
                Priority.Knockout,
                componentMethod
            );

        ClassCheck concreteComponent =
            Class(
                Priority.Knockout,
                Implements(
                    Priority.Knockout,
                    component
                ),
                Method(
                    Priority.Knockout,
                    Overrides(
                        Priority.Knockout,
                        componentMethod
                    )
                )
            );

        MethodCheck baseDecoratorMethod =
            Method(
                Priority.Knockout,
                Overrides(
                    Priority.Knockout,
                    componentMethod
                ),
                Uses(
                    Priority.Knockout,
                    componentMethod
                )
            );

        FieldCheck baseDecoratorField =
            Field(
                Priority.Knockout,
                Type(
                    Priority.Knockout,
                    component
                )
            );

        ClassCheck baseDecorator =
            AbstractClass(
                Priority.Knockout,
                Implements(
                    Priority.Knockout,
                    component
                ),
                baseDecoratorField,
                Constructor(
                    Priority.Knockout,
                    Uses(
                        Priority.Knockout,
                        baseDecoratorField
                    ),
                    Parameters(
                        Priority.Knockout,
                        Type(
                            Priority.Knockout,
                            component
                        )
                    )
                ),
                baseDecoratorMethod
            );

        ClassCheck concreteDecorator =
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
                ),
                Method(Priority.Mid) //TODO add to docs
            );

        ClassCheck client =
            Class(
                Priority.Mid,
                Uses(
                    Priority.Mid,
                    component
                ),
                Creates(
                    Priority.Mid,
                    concreteDecorator
                ),
                Creates(
                    Priority.Mid,
                    concreteComponent
                )
            );

        yield return component;
        yield return concreteComponent;
        yield return baseDecorator;
        yield return concreteDecorator;
        yield return client;
    }

    public List<IInstruction> GenerateStepsList()
    {
        throw new NotImplementedException();
    }
}
