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
    /// <inheritdoc />
    public override MethodCheck HasMethodCheck()
    {
        return Method(Priority.High,
            "1b. Has an abstract method.",
            Modifiers(Priority.High,
                Modifier.Abstract
            )
        );
    }

    /// <inheritdoc />
    public override ClassCheck HasInterfaceOrAbstractClassWithMethodCheck(MethodCheck methodInImplementation)
    {
        return AbstractClass(Priority.Knockout, "1. Implementation Abstract Class",
            methodInImplementation
        );
    }

    /// <inheritdoc />
    public override ClassCheck ConcreteImplementationCheck(CheckBase implementationCheck, MethodCheck methodInImplementation)
    {
        return Class(Priority.Knockout,
            "3. Concrete Implementation Class",
            Inherits(Priority.Knockout,
                "3a. Inherits from the Implementation abstract class.",
                implementationCheck
            ),
            Method(Priority.High,
                Overrides(Priority.High,
                    "3b. Overrides a method.",
                    methodInImplementation
                )
            )
        );
    }
}
