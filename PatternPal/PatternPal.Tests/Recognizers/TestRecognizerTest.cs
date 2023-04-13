using System.Collections.Generic;
using System.Linq;
using System.Text;
using PatternPal.Tests.Recognizers.TestRecognizers;

using NUnit.Framework;
using SyntaxTree;

namespace PatternPal.Tests.Recognizers
{
    public  class TestRecognizerTest
    {
        [Test]
        [TestCase("TestRecognizerTest1", "Class1", 100, 100)] // Correct optional and knockout returns 100
        [TestCase("TestRecognizerTest2", "Class1", 0, 0)] // Incorrect knockout and correct optional returns 0
        [TestCase("TestRecognizerTest3", "Class1", 1, 100)] // Incorrect optional and correct knockout does not return 0
        public void TestRecognizer_ReturnsCorrectScore(string directory, string filename, int minScore, int maxScore)
        {
            var recognizer = new TestRecognizer();
            var filesAsString = FileUtils.FilesToString($"{directory}\\");
            var nameSpaceName = $"PatternPal.Tests.TestClasses.{directory}";

            var graph = new SyntaxGraph();
            var i = 0;

            foreach (var s in filesAsString)
            {
                graph.AddFile(s, i++.ToString());
            }

            graph.CreateGraph();

            var nodes = graph.GetAll();
            var result = recognizer.Recognize(nodes[nameSpaceName + "." + filename]);

            Assert.That(result.GetScore(), Is.InRange(minScore, maxScore));
        }
    }
}
