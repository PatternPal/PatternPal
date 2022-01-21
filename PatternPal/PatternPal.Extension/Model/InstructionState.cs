using System.Collections.Generic;
using PatternPal.StepByStep.Abstractions;
using SyntaxTree.Abstractions.Entities;

namespace PatternPal.Extension.Model
{
    public class InstructionState : Dictionary<string, IEntity>, IInstructionState
    {
    }
}
