#region

using System.Collections.Generic;

using SyntaxTree.Abstractions.Entities;

#endregion

namespace PatternPal.StepByStep.Abstractions
{
    public interface IInstructionState : IDictionary< string, IEntity >
    {
    }
}
