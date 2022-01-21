using System.Collections.Generic;
using SyntaxTree.Abstractions.Entities;

namespace IDesign.StepByStep.Abstractions
{
    public interface IInstructionState : IDictionary<string, IEntity>
    {
        
    }
}
