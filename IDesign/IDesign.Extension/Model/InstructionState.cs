using System.Collections.Generic;
using IDesign.StepByStep.Abstractions;
using SyntaxTree.Abstractions.Entities;

namespace IDesign.Extension.Model
{
    public class InstructionState : Dictionary<string, IEntity>, IInstructionState
    {
    }
}
