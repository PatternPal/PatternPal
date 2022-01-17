using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.Checks.Entities;
using IDesign.Recognizers.Models.Output;
using IDesign.StepByStep.Abstractions;
using IDesign.StepByStep.Models;
using IDesign.StepByStep.Resources.Instructions;
using SyntaxTree.Abstractions;
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

            list.Add(new ComplexInstruction
                (
                "Strategy Abstract",
                StrategyInstructions._1,
                new List<IInstructionCheck> { new AbstractCheck() },
                "strategy.abstract"
                ));
            
            list.Add(new ComplexInstruction
                (
                "Strategy Subclass Abstract", 
                StrategyInstructions._2,
                new List<IInstructionCheck> { new CheckIfClassIsSubclassOfAbstractClass() },
                "strategy.abstract.subclass"
                ));

            list.Add(new ComplexInstruction
                (
                "Strategy Abstract Perform",
                StrategyInstructions._3,
                new List<IInstructionCheck> { new CheckForPerformMethod() },
                "strategy.abstract"
                ));

            list.Add(new ComplexInstruction
                (
                "Strategy Interface Behaviour",
                StrategyInstructions._4,
                new List<IInstructionCheck> { new InterfaceCheck(), new CheckMethodCount() },
                "strategy.interface"
                ));
        }

        private class ComplexInstruction : SimpleInstruction, IFileSelector
        {
            private readonly string _fileId;
            public string FileId => _fileId;

            public ComplexInstruction(string title, string description, List<IInstructionCheck> checks, string fileId) : base(title, description, checks)
            {
                _fileId = fileId;
            }
        }
        
        private class AbstractCheck : IInstructionCheck
        {
            public ICheckResult Correct(IInstructionState state)
            {
                if (!state.ContainsKey("strategy.abstract")) return new CheckResult("", FeedbackType.Incorrect, null);
                
                var entity = state["strategy.abstract"];

                return new EntityCheck()
                    .Modifiers(Modifiers.Abstract)
                    .Check(entity);
            }
        }

        private class CheckIfClassIsSubclassOfAbstractClass : IInstructionCheck
        {
            public ICheckResult Correct(IInstructionState state)
            {
                if (!state.ContainsKey("strategy.abstract.subclass")) return new CheckResult("", FeedbackType.Incorrect, null);

                var entity = state["strategy.abstract.subclass"];
                var strategyAbstract = state["strategy.abstract"];

                return new EntityCheck()
                    .Custom(
                        m => m.GetRelations().Any(x => x.GetRelationType() == RelationType.Extends && x.GetDestination().GetName() == strategyAbstract.GetName()),
                        new ResourceMessage("StrategyCheckIfClassIsSubclassOfAbstractClass")
                        )
                    .Check(entity);
            }
        }

        private class CheckForPerformMethod : IInstructionCheck
        {
            public ICheckResult Correct(IInstructionState state)
            {
                if (!state.ContainsKey("strategy.abstract")) return new CheckResult("", FeedbackType.Incorrect, null);

                var entity = state["strategy.abstract"];

                return new EntityCheck()
                    .Custom(
                    m => m.GetAllMethods().Any(x => x.GetName().Contains("Perform") && x.GetModifiers().Contains(Modifiers.Abstract)),
                    new ResourceMessage("StrategyCheckForPerformMethod")
                    )
                    .Check(entity);
            }
        }

        private class InterfaceCheck : IInstructionCheck
        {
            public ICheckResult Correct(IInstructionState state)
            {
                if (!state.ContainsKey("strategy.interface")) return new CheckResult("", FeedbackType.Incorrect, null);

                var entity = state["strategy.interface"];

                return new EntityCheck()
                    .Type(EntityType.Interface)
                    .Check(entity);
            }
        }

        private class CheckMethodCount : IInstructionCheck
        {
            public ICheckResult Correct(IInstructionState state)
            {
                if (!state.ContainsKey("strategy.interface")) return new CheckResult("", FeedbackType.Incorrect, null);

                var entity = state["strategy.interface"];

                return new EntityCheck()
                    .Custom(
                    m => m.GetAllMethods().Count() > 0,
                    new ResourceMessage("StrategyCheckMethodCount")
                    )
                    .Check(entity);
            }
        }
    }
}
