using System.Collections.Generic;

namespace IDesign.StepByStep.Abstractions
{
    /// <summary>
    /// Interface which should be implemented by the pattern instruction sets
    /// </summary>
    public interface IInstructionSet
    {
        /// <summary>
        /// The name of the instruction set
        /// </summary>
        string Name { get; }

        /// <summary>
        /// List that contains all instructions for this instruction set
        /// </summary>
        IEnumerable<IInstruction> Instructions { get; }
    }
}
