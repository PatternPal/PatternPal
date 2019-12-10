using System.Linq;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using NUnit.Framework;

namespace IDesign.Tests.Recognizers
{
    public class SingletonRecognizerTest
    {
        [Test]
        [TestCase("SingleTonTestCase1.cs", 100)]
        [TestCase("SingleTonTestCase2.cs", 42)]
        [TestCase("SingleTonTestCase3.cs", 28)]
        [TestCase("SingleTonTestCase4.cs", 100)]
        [TestCase("SingleTonTestCase6.cs", 100)]
        [TestCase("SingleTonTestCase7.cs", 57)]
        public void SingletonRecognizer_Returns_Correct_Score(string filename, int score)
        {
            var singleton = new SingletonRecognizer();
            var code = FileUtils.FileToString("Singleton\\" + filename);
            var testGraph = EntityNodeUtils.CreateEntityNodeGraphFromOneFile(code);
            var testNode = testGraph.Values.First();

            var result = singleton.Recognize(testNode);

            Assert.AreEqual(score, result.GetScore());
        }
    }
}