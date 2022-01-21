using System.Collections.Generic;

namespace PatternPal.StepByStep.Abstractions
{
    public interface IInstruction
    {
        /// <summary>
        /// The title of the instruction
        /// </summary>
        string Title { get; }
        
        /// <summary>
        /// Text that contains the instruction
        /// </summary>
        string Description { get; }

        /// <summary>
        /// All checks that need to be passed to continue
        /// </summary>
        IEnumerable<IInstructionCheck> Checks { get; }
    }
}
