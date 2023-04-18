using PatternPal.Recognizers.Recognizers;

using NUnit.Framework;
using SyntaxTree;

namespace PatternPal.Tests.Recognizers
{
    [TestFixture]
    public class FactoryMethodRecognizerTest
    {
        [Ignore("Old Tests, ignored for CI")]
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
            var nameSpaceName = $"PatternPal.Tests.TestClasses.{directory}";


            var graph = new SyntaxGraph();
            var i = 0;
            foreach (var s in filesAsString)
            {
                graph.AddFile(s, i++.ToString());
            }

            graph.CreateGraph();

            var entityNodes = graph.GetAll();
            var result = factoryMethodRecognizer.Recognize(entityNodes[nameSpaceName + "." + filename]);

            Assert.That(result.GetScore(), Is.InRange(minScore, maxScore));
        }
    }
}
