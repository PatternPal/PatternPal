using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using IDesign.StepByStep.Abstractions;
using IDesign.StepByStep.InstructionSets;

namespace IDesign.StepByStep
{
    public static class InstructionSetsCreator
    {
        public static readonly ImmutableList<IInstructionSet> InstructionSets = ImmutableList.Create<IInstructionSet>(
            new StrategyInstructionSet()
        );
    }
}
