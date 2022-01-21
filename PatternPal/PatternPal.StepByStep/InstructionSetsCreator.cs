using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using PatternPal.StepByStep.Abstractions;
using PatternPal.StepByStep.InstructionSets;

namespace PatternPal.StepByStep
{
    public static class InstructionSetsCreator
    {
        public static readonly ImmutableList<IInstructionSet> InstructionSets = ImmutableList.Create<IInstructionSet>(
            new StrategyInstructionSet()
        );
    }
}
