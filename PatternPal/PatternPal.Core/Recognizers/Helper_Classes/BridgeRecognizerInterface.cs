#region
using static PatternPal.Core.Checks.CheckBuilder;
#endregion

namespace PatternPal.Core.Recognizers.Helper_Classes;

/// <summary>
/// The Bridge pattern recognizers when the Implementation is an interface
/// </summary>
internal class BridgeRecognizerInterface : BridgeRecognizerParent
{
    public BridgeRecognizerInterface()
    {
        Initialize();
    }

    /// <inheritdoc />
    public override MethodCheck HasMethodCheck()
    {
        return Method(Priority.High, "1b. Has a method.");
    }

    /// <inheritdoc />
    public override CheckBase ImplementationCheckBase()
    {

        return Interface(Priority.Knockout,
            "1. Implementation Interface",
            ImplementationMethod
        );
    }

    /// <inheritdoc />
    public override ClassCheck ConcreteImplementationCheck()
    {
        return Class(Priority.Knockout,
            "3. Concrete Implementation class",
            Implements(Priority.Knockout,
                "3a. Implements the Implementation interface.",
                ImplementationClass
            )
        );
    }
}
