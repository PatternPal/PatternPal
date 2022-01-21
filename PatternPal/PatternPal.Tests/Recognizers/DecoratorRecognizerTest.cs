using PatternPal.Recognizers.Recognizers;
using PatternPal.Tests.Utils;
using NUnit.Framework;
using SyntaxTree;

namespace PatternPal.Tests.Recognizers
{
    public class DecoratorRecognizerTest
    {
        [TestCase("DecoratorTest1", "Decorator", 80, 100)]
        [TestCase("DecoratorTest2", "Decorator", 0, 79)]
        [TestCase("DecoratorTest3", "Decorator", 0, 79)]
        [TestCase("DecoratorTest4", "Decorator", 0, 79)]
        [TestCase("DecoratorTest5", "Decorator", 0, 79)]
        public void DecoratorRecognizer_Returns_Correct_Score(
            string directory,
            string filename,
            int minScore,
            int maxScore
        )
        {
            var decorator = new DecoratorRecognizer();
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
            var result = decorator.Recognize(entityNodes[nameSpaceName + "." + filename]);

            Assert.That(result.GetScore(), Is.InRange(minScore, maxScore));
        }
    }
}
