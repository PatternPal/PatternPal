#region

using PatternPal.Core.StepByStep;
using PatternPal.SyntaxTree.Models;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided files or project implements the singleton pattern
/// </summary>
/// <remarks>
/// Requirements to fulfill the pattern:<br/>
/// a) has no public/internal constructor<br/>
/// b) has at least one private/protected constructor<br/>
/// c) has a static, private field with the same type as the class<br/>
/// d0) has a static, public/internal method that acts as a constructor in the following way<br/>
///     d1) if called and there is no instance saved in the private field, then it calls the private constructor<br/>
///     d2) if called and there is an instance saved in the private field it returns this instance<br/>
/// <br/>
/// Optional requirement client:<br/>
/// a) calls the method that acts as a constructor of the singleton class<br/>
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
                        componentMethod
                    )
                ),
                Method(Priority.Mid)
            );

        ClassCheck client =
            Class(
                Priority.Mid,
                Uses(
                    Priority.Mid,
                    component
                )
            );
    }

    public List<IInstruction> GenerateStepsList()
    {
        throw new NotImplementedException();
    }
}
