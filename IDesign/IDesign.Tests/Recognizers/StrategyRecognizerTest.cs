using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.Recognizers
{
    class StrategyRecognizerTest
    {
        [TestCase("StateTest1", "Doneness", 100)]
        [TestCase("StateTest1", "Medium", 0)]
        [TestCase("StateTest1", "MediumRare", 0)]
        [TestCase("StateTest1", "Rare", 0)]
        [TestCase("StateTest1", "Ruined", 0)]
        [TestCase("StateTest1", "Uncooked", 0)]
        [TestCase("StateTest1", "Steak", 100)]
        [TestCase("StateTest2", "Account", 100)]
        [TestCase("StateTest2", "GoldState", 0)]
        [TestCase("StateTest2", "RedState", 0)]
        [TestCase("StateTest2", "SilverState", 0)]
        [TestCase("StateTest2", "State", 100)]
        [TestCase("StateTest3", "State", 100)]
        [TestCase("StateTest3", "Context", 100)]
        [TestCase("StateTest3", "ConcreteStateA", 0)]
        [TestCase("StateTest3", "ConcreteStateB", 0)]
        [TestCase("StrategyTest1", "CookingMethod", 100)]
        [TestCase("StrategyTest1", "CookStrategy", 100)]
        [TestCase("StrategyTest1", "DeepFrying", 100)]
        [TestCase("StrategyTest1", "OvenBaking", 100)]
        [TestCase("StrategyTest2", "ConcreteStrategyA", 66)]
        [TestCase("StrategyTest2", "ConcreteStrategyB", 66)]
        [TestCase("StrategyTest2", "Context", 100)]
        [TestCase("StrategyTest2", "IStrategy", 50)]
        [TestCase("StrategyTest2", "ConcreteStrategyA", 100)]
        [TestCase("StrategyTest2", "ConcreteStrategyB", 100)]
        [TestCase("StrategyTest2", "Context", 100)]
        [TestCase("StrategyTest2", "Strategy", 100)]
        public void StateRecognizer_Returns_Correct_Score(string directory, string filename, int score)
        {
            var state = new StrategyRecognizer();
            var filesAsString = FileUtils.FilesToString($"{directory}\\");
            var nameSpaceName = $"IDesign.Tests.TestClasses.{directory}";
            var entityNodes = EntityNodeUtils.CreateEntityNodeGraph(filesAsString);
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.GetEdgesOfEntityNode();
            var result = state.Recognize(entityNodes[nameSpaceName + "." + filename]);

            Assert.AreEqual(score, result.GetScore());
        }
    }
}
