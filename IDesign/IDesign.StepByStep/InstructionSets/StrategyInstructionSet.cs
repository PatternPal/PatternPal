using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using IDesign.Recognizers;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.Checks.Entities;
using IDesign.Recognizers.Models.Checks.Members;
using IDesign.Recognizers.Models.Output;
using IDesign.StepByStep.Abstractions;
using IDesign.StepByStep.Models;
using IDesign.StepByStep.Resources.Instructions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Models;
using static IDesign.Core.Resources.DesignPatternNameResources;

namespace IDesign.StepByStep.InstructionSets
{
    public class StrategyInstructionSet : IInstructionSet
    {
        public string Name => "StrategyInstruction";
        public IEnumerable<IInstruction> Instructions { get; }

        public StrategyInstructionSet()
        {
            var list = new List<IInstruction>();
            Instructions = list;

            list.Add(
                new ComplexInstruction
                (
                    "Strategy Abstract",
                    StrategyInstructions._1,
                    new List<IInstructionCheck> { new AbstractCheck() },
                    "strategy.abstract"
                )
            );

            list.Add(
                new ComplexInstruction
                (
                    "Strategy Subclass Abstract",
                    StrategyInstructions._2,
                    new List<IInstructionCheck> { new CheckIfClassIsSubclassOfAbstractClass() },
                    "strategy.abstract.subclass"
                )
            );

            list.Add(
                new SimpleInstruction(
                    "Strategy Abstract Perform",
                    StrategyInstructions._3,
                    new List<IInstructionCheck> { new CheckForPerformMethod() }
                )
            );

            list.Add(
                new ComplexInstruction
                (
                    "Strategy Interface Behaviour",
                    StrategyInstructions._4,
                    new List<IInstructionCheck> { new InterfaceCheck(), new CheckMethodCount() },
                    "strategy.interface"
                )
            );

            list.Add(
                new SimpleInstruction(
                    "Strategy Abstract Property",
                    StrategyInstructions._5,
                    new List<IInstructionCheck>() { new PropertyCheck() }
                )
            );

            list.Add(
                new SimpleInstruction(
                    "Strategy Abstract Method",
                    StrategyInstructions._6,
                    new List<IInstructionCheck>() { new MethodCalledThroughBehaviourCheck() }
                )
            );

            list.Add(
                new ComplexInstruction(
                    "Strategy Interface Implementation",
                    StrategyInstructions._7,
                    new List<IInstructionCheck>() { new CheckIfClassIsSubclassOfInterface() },
                    "strategy.interface.subclass"
                )
            );

            list.Add(
                (new SimpleInstruction(
                    "Strategy Constructor Behaviour Declaration",
                    StrategyInstructions._8,
                    new List<IInstructionCheck>() { new ConstructorInstantiatesBehaviourCheck() }
                ))
            );
        }

        private class ComplexInstruction : SimpleInstruction, IFileSelector
        {
            private readonly string _fileId;
            public string FileId => _fileId;

            public ComplexInstruction(
                string title,
                string description,
                List<IInstructionCheck> checks,
                string fileId
            ) : base(title, description, checks)
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
                if (!state.ContainsKey("strategy.abstract.subclass"))
                    return new CheckResult("", FeedbackType.Incorrect, null);

                var entity = state["strategy.abstract.subclass"];
                var strategyAbstract = state["strategy.abstract"];

                return new EntityCheck()
                    .Custom(
                        m => m.GetRelations().Any(
                            x => x.GetRelationType() == RelationType.Extends &&
                                 x.GetDestination().GetName() == strategyAbstract.GetName()
                        ),
                        new ResourceMessage("StrategyInstructionCheckIfClassIsSubclassOfAbstractClass", strategyAbstract.GetName())
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
                    .Any.Method()
                        .Custom(
                            m => m.GetName().Contains("Perform"),
                            new ResourceMessage("StrategyInstructionCheckForPerformMethod")
                        )
                    .Build()
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
                    .Any.Method("StrategyInstructionMethod")
                    .Build()
                    .Check(entity);
            }
        }

        private class PropertyCheck : IInstructionCheck
        {
            public ICheckResult Correct(IInstructionState state)
            {
                if (!state.ContainsKey("strategy.abstract")) return new CheckResult("", FeedbackType.Incorrect, null);

                var entity = state["strategy.abstract"];

                var interfaceEntity = state["strategy.interface"];
                return new EntityCheck()
                    .Any.Field(variants: true)
                    .Type(interfaceEntity)
                    .Build()
                    .Check(entity);
            }
        }

        private class MethodCalledThroughBehaviourCheck : IInstructionCheck
        {
            public ICheckResult Correct(IInstructionState state)
            {
                if (!state.ContainsKey("strategy.abstract")) return new CheckResult("", FeedbackType.Incorrect, null);

                var entity = state["strategy.abstract"];
                var interfaceEntity = state["strategy.interface"];
                return
                    new EntityCheck() //NOTE: if there's a different method called in the body this might be seen as true (while it should not be)
                        .Any.Method()
                        .Custom(
                            x =>
                            {
                                var expression = x.GetBody()?.DescendantNodes().OfType<InvocationExpressionSyntax>()
                                    .FirstOrDefault();
                                if (expression == default) return false;
                                var split = expression.Expression.ToString().Split('.').ToList();
                                split.Remove("this");
                                split.Remove("base");
                                if (split.Count() <= 1) return false;


                                var field = entity.GetAllFields().FirstOrDefault(f => f.GetName().Equals(split[0]));
                                if (field == null) return false;
                                return field.GetFieldType().ToString().Equals(interfaceEntity.GetName());
                            },
                            new ResourceMessage("StrategyInstructionMethodCalledThroughBehaviourCheck", interfaceEntity.GetName())
                        )
                        .Build()
                        .Check(entity);
            }
        }

        private class CheckIfClassIsSubclassOfInterface : IInstructionCheck
        {
            public ICheckResult Correct(IInstructionState state)
            {
                if (!state.ContainsKey("strategy.interface.subclass"))
                    return new CheckResult("", FeedbackType.Incorrect, null);

                var entity = state["strategy.interface.subclass"];
                var strategyInterface = state["strategy.interface"];

                return new EntityCheck()
                    .Custom(
                        m =>
                        {
                            return m.GetRelations().Any(
                                x => x.GetRelationType() == RelationType.Implements &&
                                     x.GetDestination().GetName() == strategyInterface.GetName()
                            );
                        },
                        new ResourceMessage("StrategyInstructionCheckIfClassIsSubclassOfInterfaceClass", strategyInterface.GetName())
                    )
                    .Check(entity);
            }
        }

        private class ConstructorInstantiatesBehaviourCheck : IInstructionCheck
        {
            public ICheckResult Correct(IInstructionState state)
            {
                if (!state.ContainsKey("strategy.abstract.subclass"))
                    return new CheckResult("", FeedbackType.Incorrect, null);

                var entity = state["strategy.abstract.subclass"];
                var create = state["strategy.interface.subclass"];
                return
                    new EntityCheck()
                        .Any.Constructor()
                            .Custom(
                                c => c.AsMethod().CheckCreationType(create.GetName()),
                                new ResourceMessage("StrategyInstructionConstructorInstantiatesBehaviour", create.GetName())
                            )
                        .Build()
                        .Check(entity);
            }
        }
    }
}
