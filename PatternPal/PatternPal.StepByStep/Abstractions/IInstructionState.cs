using System.Collections.Generic;
using SyntaxTree.Abstractions.Entities;

namespace PatternPal.StepByStep.Abstractions
{
    public interface IInstructionState : IDictionary<string, IEntity>
    {
        
    }
}
