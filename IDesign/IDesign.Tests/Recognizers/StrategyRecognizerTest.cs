using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using NUnit.Framework;

namespace IDesign.Tests.Recognizers
{
    class StrategyRecognizerTest
    {
        [TestCase("StateTest1", "Doneness", 71)]
        [TestCase("StateTest2", "State", 75)]
        [TestCase("StateTest3", "State", 75)]
        [TestCase("StrategyTest1", "CookStrategy", 100)]
        [TestCase("StrategyTest2", "IStrategy", 70)]
        [TestCase("StrategyTest3", "Strategy", 100)]
        public void StrategyRecognizer_Returns_Correct_Score(string directory, string filename, int score)
        {
            var strategy = new StrategyRecognizer();
            var filesAsString = FileUtils.FilesToString($"{directory}\\");
            var nameSpaceName = $"IDesign.Tests.TestClasses.{directory}";
            var entityNodes = EntityNodeUtils.CreateEntityNodeGraph(filesAsString);
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.GetEdgesOfEntityNode();
            var result = strategy.Recognize(entityNodes[nameSpaceName + "." + filename]);

            Assert.AreEqual(score, result.GetScore());
        }
    }
}
