using System.Collections.Generic;
using PatternPal.Core.Recognizers;
using PatternPal.Core.StepByStep;

namespace PatternPal.StepByStep.Abstractions
{
    /// <summary>
    /// Interface which should be implemented by the pattern instruction sets
    /// </summary>
    public interface IInstructionSet
    {
        /// <summary>
        /// Design pattern.
        /// </summary>
        string Pattern { get; }

        /// <summary>
        /// Recognizer corresponding to the pattern.
        /// </summary>
        IRecognizer Recognizer {get; }

        /// <summary>
        /// List that contains all instructions for this instruction set.
        /// </summary>
        List<IInstruction> Steps { get; }

        /// <summary>
        /// Retrieves the current instruction based on the step.
        /// </summary>
        IInstruction ObtainCurrentInstruction();

        /// <summary>
        /// The current step.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Increment the indexer.
        /// </summary>
        void NextStep();

        /// <summary>
        /// Decrease the indexer.
        /// </summary>
        void PreviousStep();
    }
}
