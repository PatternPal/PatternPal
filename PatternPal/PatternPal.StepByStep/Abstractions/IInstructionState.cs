#region

using System.Collections.Generic;

using PatternPal.SyntaxTree.Abstractions.Entities;

#endregion

namespace PatternPal.StepByStep.Abstractions
{
    public interface IInstructionState : IDictionary< string, IEntity >
    {
    }
}
