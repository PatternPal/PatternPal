using static PatternPal.Core.Checks.CheckBuilder;

namespace PatternPal.Core.Recognizers
{
    internal class AdapterRecognizer : IRecognizer
    {
        /// <inheritdoc />
        public string Name => "Adapter";

        /// <inheritdoc />
        public Recognizer RecognizerType => Recognizer.Adapter;

        readonly AdapterRecognizerParent _isInterface = new AdapterRecognizerInterface();
        readonly AdapterRecognizerParent _isAbstractClass = new AdapterRecognizerAbstractClass();

        public IEnumerable<ICheck> Create()
        {
            yield return Any(Priority.Low,
                _isAbstractClass.Checks().Concat(_isInterface.Checks()).ToArray());
        }
    }
}
