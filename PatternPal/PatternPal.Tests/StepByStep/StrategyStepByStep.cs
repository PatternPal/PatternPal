using System.Collections.Generic;
using System.Linq;
using PatternPal.Recognizers.Abstractions;
using PatternPal.StepByStep.Abstractions;
using PatternPal.StepByStep.InstructionSets;
using PatternPal.StepByStep.Resources.Instructions;
using NUnit.Framework;

namespace PatternPal.Tests.StepByStep
{
    public class StrategyStepByStep
    {
        private StrategyInstructionSet _set;
        private IInstructionState _state;

        [SetUp]
        public void Setup()
        {
            _set = new StrategyInstructionSet();
            var dic = new Dictionary<string, string>
            {
                { "strategy.abstract", "Duck.cs" },
                { "strategy.abstract.subclass", "VeryCoolDuck.cs" },
                { "strategy.interface", "IBehaviour.cs" },
                { "strategy.interface.subclass", "VeryCoolBehaviour.cs" }
            };
            _state = new TestInstructionState(dic, "StrategyStepByStepTest");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        public void Strategy_StepN(int n)
        {
            var instruction = _set.Instructions.ToList()[n - 1];
            foreach (var check in instruction.Checks)
            {
                var result = check.Correct(_state);
                Assert.AreEqual(FeedbackType.Correct, result.GetFeedbackType(), result.GetFeedback()?.GetKey());
            }
        }
    }
}
