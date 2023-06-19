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
            Modifiers(Priority.High,
                Modifier.Abstract
            )
        );
    }

    /// <inheritdoc />
    public override ClassCheck HasInterfaceOrAbstractClassWithMethodCheck(MethodCheck methodInImplementation)
    {
        return AbstractClass(Priority.Knockout,
            methodInImplementation
        );
    }

    /// <inheritdoc />
    public override ClassCheck ConcreteImplementationCheck(CheckBase implementationCheck, MethodCheck methodInImplementation)
    {
        return Class(Priority.Knockout,
            Inherits(Priority.Knockout,
                implementationCheck
            ),
            Method(Priority.High,
                Overrides(Priority.High,
                    methodInImplementation
                )
            )
        );
    }
}
