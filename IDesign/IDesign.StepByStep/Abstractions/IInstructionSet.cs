using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.StepByStep.Abstractions
{
    /// <summary>
    /// An interface which should be implemented by the pattern instruction sets
    /// </summary>
    public interface IInstructionSet
    {
        /// <summary>
        /// The name of the instruction
        /// </summary>
        string InstructionName { get; }
    }
}
