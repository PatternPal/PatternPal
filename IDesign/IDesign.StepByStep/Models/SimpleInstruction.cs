using System.Collections.Generic;
using System.Linq;
using IDesign.StepByStep.Abstractions;

namespace IDesign.StepByStep.Models
{
    public class SimpleInstruction : IInstruction
    {
        private readonly string _title;
        private readonly string _description;

        public SimpleInstruction(string title, string description)
        {
            _title = title;
            _description = description;
        }

        public string Title => _title ?? "Instruction unavailable";
        public string Description => _description ?? "";
        
        public virtual IEnumerable<IInstructionCheck> Checks => Enumerable.Empty<IInstructionCheck>();
    }
}
