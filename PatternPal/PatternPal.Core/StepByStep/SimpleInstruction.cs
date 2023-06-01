using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternPal.Core.StepByStep
{
    public class SimpleInstruction : IInstruction
    {
        public string Requirement { get; }
        public string Description { get; }
        public List<ICheck> Checks { get; }
        public string FileId { get; set; }

        public SimpleInstruction(string requirement, string description, List<ICheck> checks)
        {
            Requirement = requirement;
            Description = description;
            Checks = checks;
        }
    }
}
