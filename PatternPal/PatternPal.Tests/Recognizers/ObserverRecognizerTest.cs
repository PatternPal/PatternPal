using PatternPal.Recognizers.Recognizers;
using PatternPal.Tests.Utils;
using NUnit.Framework;
using SyntaxTree;

namespace PatternPal.Tests.Recognizers
{
    public class ObserverRecognizerTest
    {
        [TestCase("ObserverTest1", "IObserver", 80, 100)]
        [TestCase("ObserverTest2", "IInvestor", 80, 100)]
        [TestCase("ObserverTest3", "IObserver", 80, 100)]
        [TestCase("ObserverTest4", "IObserver", 0, 79)]
        [TestCase("ObserverTest5", "IObserver", 0, 79)]
        public void ObserverRecognizer_Returns_Correct_Score(
            string directory,
            string filename,
            int minScore,
            int maxScore
        )
        {
            var observer = new ObserverRecognizer();
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
            var result = observer.Recognize(entityNodes[nameSpaceName + "." + filename]);

            Assert.That(result.GetScore(), Is.InRange(minScore, maxScore));
        }
    }
}
