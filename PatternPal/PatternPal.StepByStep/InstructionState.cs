#region

using System.Collections.Generic;

using PatternPal.StepByStep.Abstractions;

using SyntaxTree.Abstractions.Entities;

#endregion

namespace PatternPal.StepByStep
{
    public class InstructionState : Dictionary< string, IEntity >,
                                    IInstructionState
    {
    }
}
