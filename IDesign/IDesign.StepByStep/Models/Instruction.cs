using System;

namespace IDesign.StepByStep.Models
{
    public class Instruction
    {
        private string _instructionText;
        /// <summary>
        /// Text that contains the instruction
        /// </summary>
        public string InstructionText
        {
            get
            {
                return _instructionText ?? "Instruction unavailable";
            }
            set
            {
                _instructionText = value;
            }
        }

        public Instruction(string instructionText)
        {
            InstructionText = instructionText;
        }
    }
}
