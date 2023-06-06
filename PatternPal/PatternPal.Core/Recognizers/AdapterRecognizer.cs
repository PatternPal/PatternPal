using PatternPal.Core.Recognizers.Helper_Classes;
using PatternPal.Core.StepByStep;
using static PatternPal.Core.Checks.CheckBuilder;

namespace PatternPal.Core.Recognizers
{
    /// <summary>
    /// A <see cref="IRecognizer"/> that is used to determine if the provided files or project implements the adapter pattern
    /// </summary>
    /// <remarks>
    /// Requirements for the Service class:<br/>
    ///     a) does not inherit from the Client Interface.<br/>
    ///     b) is used by the Adapter class<br/>
    /// <br/>
    /// Requirements for the Client class:<br/>
    ///     a) has created an object of the type Adapter<br/>
    ///     b) has used a method of the Service via the Adapter<br/>
    /// <br/>
    /// Requirements for the Client interface:<br/>
    ///     a) is an interface/abstract class<br/>
    ///     b) is inherited/implemented by an Adapter<br/>
    ///     c) contains a method<br/>
    ///         i) if it is an abstract class the method should be abstract or virtual<br/>
    /// <br/>
    /// Requirements for the Adapter class:<br/>
    ///     a) inherits/implements the Client Interface<br/>
    ///     b) creates an Service object<br/>
    ///     c) contains a private field in which the Service is stored<br/>
    ///     d) does not return an instance of the Service<br/>
    ///     e) a method uses the Service class<br/>
    ///     f) every method uses the Service class<br/>
    /// </remarks>
    internal class AdapterRecognizer : IRecognizer
    {
        /// <inheritdoc />
        public string Name => "Adapter";

        /// <inheritdoc />
        public Recognizer RecognizerType => Recognizer.Adapter;

        readonly AdapterRecognizerParent _isInterface = new AdapterRecognizerInterface();
        readonly AdapterRecognizerParent _isAbstractClass = new AdapterRecognizerAbstractClass();

        /// <summary>
        /// A method which creates a lot of <see cref="ICheck"/>s that each adheres to the requirements a adapter pattern needs to have implemented.
        /// It returns the requirements in a tree structure stated per class.
        /// </summary>
        public IEnumerable<ICheck> Create()
        {
            yield return Any(
                Priority.Low,
                All(
                    Priority.Low,
                    _isAbstractClass.Checks()
                ),
                All(
                Priority.Low,
                    _isInterface.Checks()
                )
            );
        }

        public List<IInstruction> GenerateStepsList()
        {
            throw new NotImplementedException();
        }
    }
}
