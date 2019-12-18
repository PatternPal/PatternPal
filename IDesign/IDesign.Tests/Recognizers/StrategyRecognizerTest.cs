using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using NUnit.Framework;

namespace IDesign.Tests.Recognizers
{
    class StrategyRecognizerTest
    {
        [TestCase("StateTest1", "Doneness", 0,79)]
        [TestCase("StateTest2", "State", 0,79)]
        [TestCase("StateTest3", "State", 0, 79)]
        [TestCase("StrategyTest1", "CookStrategy",80, 100)]
        [TestCase("StrategyTest2", "IStrategy", 80, 100)]
        [TestCase("StrategyTest3", "Strategy", 80,100)]
        public void StrategyRecognizer_Returns_Correct_Score(string directory, string filename,int minScore, int maxScore)
        {
            var strategy = new StrategyRecognizer();
            var filesAsString = FileUtils.FilesToString($"{directory}\\");
            var nameSpaceName = $"IDesign.Tests.TestClasses.{directory}";
            var entityNodes = EntityNodeUtils.CreateEntityNodeGraph(filesAsString);
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.GetEdgesOfEntityNode();
            var result = strategy.Recognize(entityNodes[nameSpaceName + "." + filename]);

           // Assert.AreEqual(score, result.GetScore());
            Assert.That(result.GetScore(), Is.InRange(minScore, maxScore));
        }
    }
}
