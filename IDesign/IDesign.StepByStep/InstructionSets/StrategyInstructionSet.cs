using System;
using System.Collections.Generic;
using System.Text;
using IDesign.StepByStep.Abstractions;
using IDesign.Core.Resources;
using static IDesign.Core.Resources.DesignPatternNameResources;

namespace IDesign.StepByStep.InstructionSets
{
    internal class StrategyInstructionSet : IInstructionSet
    {
        public string InstructionName => Strategy;
    }
}
