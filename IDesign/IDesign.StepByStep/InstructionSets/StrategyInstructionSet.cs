using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.Checks.Entities;
using IDesign.Recognizers.Models.Output;
using IDesign.StepByStep.Abstractions;
using IDesign.StepByStep.Models;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Models;
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
            
            list.Add(new StrategyInstructionInterface("Strategy interface", "Create an interface or abstract class which will be the strategy class."));
            list.Add(new SimpleInstruction("Strategy Context", "Create a class that implements the interface/abstract class you've just created (context class)."));
            list.Add(new SimpleInstruction("Concrete Strategy", "Create a class which will be the concrete strategy class."));
            list.Add(new SimpleInstruction("Concrete Strategy", "Make a field/property in the concrete strategy class with the strategy class as its type. Make the modifier of the field/property private."));
        }
        
        private class StrategyInstructionInterface : SimpleInstruction, IFileSelector
        {
            public StrategyInstructionInterface(string title, string description) : base(title, description) {
            }

            public override IEnumerable<IInstructionCheck> Checks => new List<IInstructionCheck>()
            {
                new AbstractCheck()
            };

            public string FileId => "strategy.interface";
        }
        
        private class AbstractCheck : IInstructionCheck
        {
            public ICheckResult Correct(IInstructionState state)
            {
                if (!state.ContainsKey("strategy.interface")) return new CheckResult("", FeedbackType.Incorrect, null);
                
                var entity = state["strategy.interface"];
                return new EntityCheck()
                    .Modifiers(Modifiers.Abstract)
                    .Check(entity);
            }
        }
    }
}
