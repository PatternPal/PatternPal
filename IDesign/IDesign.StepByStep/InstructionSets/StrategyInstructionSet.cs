using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IDesign.StepByStep.Abstractions;
using IDesign.StepByStep.Models;
using IDesign.StepByStep.Resources.Instructions;
using static IDesign.Core.Resources.DesignPatternNameResources;

namespace IDesign.StepByStep.InstructionSets
{
    public class StrategyInstructionSet : IInstructionSet
    {
        public string Name => Strategy;
        public LinkedList<Instruction> Instructions { get; set; }

        public StrategyInstructionSet()
        {
            SetInstructions();
        }

        public void SetInstructions()
        {
            Instructions = new LinkedList<Instruction>();
            var resourceSet = 
                StrategyInstructions.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true)
                    .OfType<DictionaryEntry>()
                    .OrderBy(i => i.Key.ToString().Length)
                    .ThenBy(i => i.Key);

            foreach (DictionaryEntry entry in resourceSet)
                Instructions.AddLast(new Instruction((string)entry.Value));
        }
    }
}
