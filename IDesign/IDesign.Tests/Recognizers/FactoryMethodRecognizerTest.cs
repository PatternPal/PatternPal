using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using NUnit.Framework;

namespace IDesign.Tests.Recognizers
{
    public class FactoryMethodRecognizerTest
    {
        [TestCase("FactoryMethodTest1", "BeerFactory", 80, 100)]
        [TestCase("FactoryMethodTest2", "CardFactory", 80, 100)]
        [TestCase("FactoryMethodTest3", "Creator", 80, 100)]
        [TestCase("FactoryMethodTest4", "Sandwich", 0, 79)]
        [TestCase("StateFactoryMethodTest1", "BeerFactory", 80, 100)]
        [TestCase("StrategyFactoryMethodTest1", "BeerFactory", 80, 100)]
        public void FactoryMethodRecognizer_Returns_Correct_Score(
            string directory,
            string filename,
            int minScore,
            int maxScore
        )
        {
            var factoryMethodRecognizer = new FactoryMethodRecognizer();
            var filesAsString = FileUtils.FilesToString($"{directory}\\");
            var nameSpaceName = $"IDesign.Tests.TestClasses.{directory}";
            var entityNodes = EntityNodeUtils.CreateEntityNodeGraph(filesAsString);
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.CreateEdgesOfEntityNode();
            var result = factoryMethodRecognizer.Recognize(entityNodes[nameSpaceName + "." + filename]);

            Assert.That(result.GetScore(), Is.InRange(minScore, maxScore));
        }
    }
}
