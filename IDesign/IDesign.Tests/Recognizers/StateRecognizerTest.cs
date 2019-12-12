using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.TestClasses.StateTest3;
using IDesign.Tests.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDesign.Tests.Recognizers
{
    class StateRecognizerTest
    {
        [TestCase("StateTest2", "Account", 100)]
        [TestCase("StateTest2", "GoldState", 100)]
        [TestCase("StateTest2", "RedState", 100)]
        [TestCase("StateTest2", "SilverState", 100)]
        [TestCase("StateTest2", "State", 100)]
        [TestCase("StateTest3", "State", 100)]
        [TestCase("StateTest3", "Context", 100)]
        [TestCase("StateTest3", "ConcreteStateA", 100)]
        [TestCase("StateTest3", "ConcreteStateB", 100)]
        [TestCase("StateTest4", "Player", 50)]
        [TestCase("StateTest4", "State", 100)]
        [TestCase("StateTest4", "DeadState", 100)]
        [TestCase("StateTest4", "HealthyState", 100)]
        [TestCase("StateTest4", "HurtState", 100)]
        [TestCase("StrategyTest1", "CookingMethod", 100)]
        [TestCase("StrategyTest1", "CookStrategy", 100)]
        [TestCase("StrategyTest1", "DeepFrying", 0)]
        [TestCase("StrategyTest1", "OvenBaking", 0)]
        public void StateRecognizer_Returns_Correct_Score(string directory, string filename, int score)
        {
            var state = new StateRecognizer();
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
