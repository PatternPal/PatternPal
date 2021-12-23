using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;
using IDesign.StepByStep.Abstractions;
using IDesign.StepByStep.Models;
using IDesign.StepByStep.Resources.Instructions;
using static IDesign.Core.Resources.DesignPatternNameResources;

namespace IDesign.StepByStep.InstructionSets
{
    public class StrategyInstructionSet : IInstructionSet
    {
        public string Name => Strategy;
        public List<Instruction> Instructions { get; set; }

        public StrategyInstructionSet()
        {
            SetInstructions();
        }

        public void SetInstructions()
        {
            Instructions = new List<Instruction>();
            foreach (DictionaryEntry entry in StrategyInstructions.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true))
                Instructions.Add(new Instruction((string)entry.Value));
        }
    }
}
