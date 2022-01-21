using System.Collections.Generic;
using System.Linq;
using PatternPal.StepByStep.Abstractions;

namespace PatternPal.StepByStep.Models
{
    public class SimpleInstruction : IInstruction
    {
        private readonly string _title;
        private readonly string _description;
        private readonly IList<IInstructionCheck> _checks;

        public SimpleInstruction(string title, string description, IList<IInstructionCheck> checks)
        {
            _title = title;
            _description = description;
            _checks = checks;
        }

        public string Title => _title ?? "Instruction unavailable";
        public string Description => _description ?? "";
        
        public virtual IEnumerable<IInstructionCheck> Checks => _checks ?? new List<IInstructionCheck>();
    }
}
