#region

using System.Collections.Generic;

using PatternPal.StepByStep.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Entities;

#endregion

namespace PatternPal.StepByStep
{
    public class InstructionState : Dictionary< string, IEntity >,
                                    IInstructionState
    {
    }
}
