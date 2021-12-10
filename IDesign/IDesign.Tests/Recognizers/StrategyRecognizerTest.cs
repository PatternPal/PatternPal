using IDesign.Recognizers.Recognizers;
using IDesign.Tests.Utils;
using NUnit.Framework;
using SyntaxTree;

namespace IDesign.Tests.Recognizers {
    class StrategyRecognizerTest {
        [TestCase("StateTest1", "Doneness", 0, 79)]
        [TestCase("StateTest2", "State", 0, 79)]
        [TestCase("StateTest3", "State", 0, 79)]
        [TestCase("StrategyTest1", "CookStrategy", 80, 100)]
        [TestCase("StrategyTest2", "IStrategy", 80, 100)]
        [TestCase("StrategyTest3", "Strategy", 80, 100)]
        [TestCase("StrategyFactoryMethodTest1", "IStrategy", 80, 100)]
        [TestCase("StateFactoryMethodTest1", "IState", 0, 79)]
        public void StrategyRecognizer_Returns_Correct_Score(
            string directory,
            string filename,
            int minScore,
            int maxScore
        ) {
            var strategy = new StrategyRecognizer();
            var filesAsString = FileUtils.FilesToString($"{directory}\\");
            var nameSpaceName = $"IDesign.Tests.TestClasses.{directory}";


            var graph = new SyntaxGraph();
            var i = 0;
            foreach (var s in filesAsString) {
                graph.AddFile(s, i++.ToString());
            }

            graph.CreateGraph();

            var entityNodes = graph.GetAll();
            var result = strategy.Recognize(entityNodes[nameSpaceName + "." + filename]);

            Assert.That(result.GetScore(), Is.InRange(minScore, maxScore));
        }
    }
}
