using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.StepByStep.Models
{
    public class Instruction
    {
        public string InstructionText { get; }

        public Instruction(string instructionText)
        {
            InstructionText = instructionText;
        }
    }
}
