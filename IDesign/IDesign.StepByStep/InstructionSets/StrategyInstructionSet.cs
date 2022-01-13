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
        public IEnumerable<IInstruction> Instructions { get; }

        public StrategyInstructionSet()
        {
            var list = new List<IInstruction>();
            Instructions = list;
            
            list.Add(new SimpleInstruction("Strategy interface", "Create an interface or abstract class which will be the strategy class."));
            list.Add(new SimpleInstruction("Strategy Context", "Create a class that implements the interface/abstract class you've just created (context class)."));
            list.Add(new SimpleInstruction("Concrete Strategy", "Create a class which will be the concrete strategy class."));
            list.Add(new SimpleInstruction("Concrete Strategy", "Make a field/property in the concrete strategy class with the strategy class as its type. Make the modifier of the field/property private."));
        }
    }
}
