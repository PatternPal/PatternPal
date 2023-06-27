#region

using PatternPal.SyntaxTree.Models;
using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers.Helper_Classes;

/// <summary>
/// The Bridge pattern recognizers when the Implementation is an abstract class
/// </summary>
internal class BridgeRecognizerAbstractClass : BridgeRecognizerParent
{
    public BridgeRecognizerAbstractClass()
    {
        Initialize();
    }

    /// <inheritdoc />
    public sealed override MethodCheck HasMethodCheck()
    {
        return Method(Priority.High,
            "1b. Has an abstract method.",
            Modifiers(Priority.High,
                Modifier.Abstract
            )
        );
    }

    /// <inheritdoc />
    public sealed override ClassCheck ImplementationCheckBase()
    {
        return AbstractClass(Priority.Knockout, "1. Implementation Abstract Class",
            ImplementationMethod
        );
    }

    /// <inheritdoc />
    public sealed override ClassCheck ConcreteImplementationCheck()
    {
        return Class(Priority.Knockout,
            "3. Concrete Implementation Class",
            Inherits(Priority.Knockout,
                "3a. Inherits from the Implementation abstract class.",
                ImplementationClass
            ),
            Method(Priority.High,
                Overrides(Priority.High,
                    "3b. Overrides a method.",
                    ImplementationMethod
                )
            )
        );
    }
}
