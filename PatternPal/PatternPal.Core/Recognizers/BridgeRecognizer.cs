#region 
using PatternPal.Core.Recognizers.Helper_Classes;
using PatternPal.Core.StepByStep;
using static PatternPal.Core.Checks.CheckBuilder;
#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided files or project implements the bridge pattern
/// </summary>
/// <remarks>
/// Requirements for the Implementation interface or abstract class:<br/>
///     a) is an interface or abstract class <br/>
///     b) has at least one (abstract) method <br/>
/// <br/>
///
/// Requirements for the Abstraction class: <br/>
///     a) has a private/protected field or property with the type of the Implementation interface or abstract class <br/>
///     b) has a method
///     c) has a method that calls a method in the Implementation interface or abstract class <br/>
///     d) has either <br/>
///         1. the property option as described in a, or has <br/>
///         2. a constructor with a parameter with the Implementation type and that uses the field as described in a, or has <br/>
///         3. a method with a parameter with the Implementation type and that uses the field as described in a  <br/>
/// <br/>
/// 
/// Requirements for the Concrete Implementations: <br/>
///     a) is an implementation of the Implementation interface or inherits from the 'Implementation' abstract class <br/>
///     b) if Implementation is an abstract class it should override it's abstract methods <br/>
/// <br/>
/// 
/// Requirements for the Refined Abstraction: <br/>
///     a) inherits from the Abstraction class <br/>
///     b) has an method <br/>
/// <br/>
///
/// Requirements for the Client class: <br/>
///     a) calls a method in the Abstraction class <br/>
///     b) creates a Concrete Implementation instance <br/>
///     c) sets the field or property in Abstraction, either through
///         1. it is a property and it sets this, or through
///         2. a constructor as described in Abstraction d2
///         3. a method as described in Abstraction d3
/// </remarks>

internal class BridgeRecognizer : IRecognizer
{
    /// <inheritdoc />
    public string Name => "Bridge";

    /// <inheritdoc />
    public Recognizer RecognizerType => Recognizer.Bridge;

    readonly BridgeRecognizerParent _abstractAbstraction = new BridgeRecognizerAbstractClass();
    readonly BridgeRecognizerParent _interfaceAbstraction = new BridgeRecognizerInterface();

    /// <inheritdoc />
    public IEnumerable<ICheck> Create()
    {
        yield return Any(
            Priority.Knockout,
            Any(Priority.Knockout, _abstractAbstraction.Checks()),
            All(Priority.Knockout, _interfaceAbstraction.Checks()));
    }
}
