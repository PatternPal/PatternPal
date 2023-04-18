using PatternPal.Recognizers.Recognizers;

using NUnit.Framework;
using SyntaxTree;

namespace PatternPal.Tests.Recognizers
{
    [TestFixture]
    internal class StateRecognizerTest
    {
        [Ignore("Old Tests, ignored for CI")]
        [TestCase("StateTest1", "Doneness", 80, 100)]
        [TestCase("StateTest2", "State", 80, 100)]
        [TestCase("StateTest3", "State", 80, 100)]
        [TestCase("StateTest4", "IState", 0, 79)]
        [TestCase("StateFactoryMethodTest1", "IState", 80, 100)]
        [TestCase("StrategyFactoryMethodTest1", "IStrategy", 0, 79)]
        public void StateRecognizer_Returns_Correct_Score(
            string directory,
            string filename,
            int minScore,
            int maxScore
        )
        {
            var state = new StateRecognizer();
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
            var result = state.Recognize(entityNodes[nameSpaceName + "." + filename]);

            Assert.That(result.GetScore(), Is.InRange(minScore, maxScore));
        }
    }
}
