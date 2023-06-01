#region

using PatternPal.Core.Recognizers;

#endregion

namespace PatternPal.Core.StepByStep
{
    /// <summary>
    /// Interface which should be implemented by the pattern instruction sets
    /// </summary>
    public class InstructionSet
    {
        public InstructionSet(
            IRecognizer recognizer,
            List< IInstruction > steps)
        {
            Recognizer = recognizer;
            Steps = steps;
        }

        /// <summary>
        /// Design pattern.
        /// </summary>
        public string Pattern => Recognizer.Name;

        /// <summary>
        /// Recognizer corresponding to the pattern.
        /// </summary>
        public IRecognizer Recognizer { get; }

        /// <summary>
        /// List that contains all instructions for this instruction set.
        /// </summary>
        public List< IInstruction > Steps { get; }
    }
}
