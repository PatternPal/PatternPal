#region
using static PatternPal.Core.Checks.CheckBuilder;
#endregion

namespace PatternPal.Core.Recognizers.Helper_Classes;

/// <summary>
/// The Bridge pattern recognizers when the Implementation is an interface
/// </summary>
internal class BridgeRecognizerInterface : BridgeRecognizerParent
{
    /// <inheritdoc />
    public override MethodCheck HasMethodCheck()
    {
        return Method(Priority.High, "1b. Has a method.");
    }

    /// <inheritdoc />
    public override InterfaceCheck HasInterfaceOrAbstractClassWithMethodCheck(MethodCheck methodInImplementation)
    {
        return Interface(Priority.Knockout,
            "1. Implementation Interface",
            methodInImplementation
        );
    }

    /// <inheritdoc />
    public override ClassCheck ConcreteImplementationCheck(CheckBase implementationCheck, MethodCheck methodInImplementation)
    {
        return Class(Priority.Knockout,
            "3. Concrete Implementation class",
            Implements(Priority.Knockout,
                "3a. Implements the Implementation interface.",
                implementationCheck
            )
        );
    }
}
