using System;
using System.Collections.Generic;
using IDesign.StepByStep.Abstractions;
using IDesign.StepByStep.InstructionSets;

namespace IDesign.StepByStep
{
    public static class InstructionSetsCreator
    {
        public static readonly List<IInstructionSet> InstructionSets = new List<IInstructionSet>
        {
            new StrategyInstructionSet(),
            new SingletonInstructionSet()
        };
    }
}
