using System;
using System.Collections.Generic;
using IDesign.StepByStep.Models;

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
        LinkedList<Instruction> Instructions { get; set; }

        /// <summary>
        /// Retrieves instructions from the corresponding .resx file and loads them into the Instructions list
        /// </summary>
        void SetInstructions();

    }
}
