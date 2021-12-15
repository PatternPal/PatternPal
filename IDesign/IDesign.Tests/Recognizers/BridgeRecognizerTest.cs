using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using NUnit.Framework;
using SyntaxTree;

namespace IDesign.Tests.Recognizers
{
    public class BridgeRecognizerTest
    {
        [TestCase("BridgeTest1", "Abstraction", 80, 100)]
        [TestCase("BridgeTest2", "CustomersBase", 80, 100)]
        [TestCase("BridgeTest3", "AbstractRemoteControl", 80, 100)]
        [TestCase("BridgeTest4", "IWeapon", 80, 100)]
        public void BridgeRecognizer_Returns_Correct_Score(string directory, string filename, int minScore, int maxScore)
        {
            var bridge = new BridgeRecognizer();
            var filesAsString = FileUtils.FilesToString($"{directory}\\");
            var nameSpaceName = $"IDesign.Tests.TestClasses.{directory}";


            var graph = new SyntaxGraph();
            var i = 0;
            foreach (var s in filesAsString) {
                graph.AddFile(s, i++.ToString());
            }

            graph.CreateGraph();

            var nodes = graph.GetAll();


            var result = bridge.Recognize(nodes[nameSpaceName + "." + filename]);

            Assert.That(result.GetScore(), Is.InRange(minScore, maxScore));
        }
    }
}
