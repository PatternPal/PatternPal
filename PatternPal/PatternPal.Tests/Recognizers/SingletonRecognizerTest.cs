using System.Linq;
using PatternPal.Recognizers.Recognizers;

using NUnit.Framework;
using PatternPal.Tests.TestClasses.Singleton;

namespace PatternPal.Tests.Recognizers
{
    [TestFixture]
    public class SingletonRecognizerTest
    {
        [Ignore("Old Tests, ignored for CI")]
        [Test]
        [TestCase("SingleTonTestCase1.cs", 80, 100)]
        [TestCase("SingleTonTestCase2.cs", 0, 79)]
        [TestCase("SingleTonTestCase3.cs", 0, 79)]
        [TestCase("SingleTonTestCase4.cs", 80, 100)]
        [TestCase("SingleTonTestCase5.cs", 80, 100)]
        [TestCase("SingleTonTestCase7.cs", 0, 79)]
        public void SingletonRecognizer_Returns_Correct_Score(string filename, int minScore, int maxScore)
        {
            var singleton = new SingletonRecognizer();
            var code = FileUtils.FileToString("Singleton\\" + filename);
            var testGraph = EntityNodeUtils.CreateEntityNodeGraphFromOneFile(code);
            var testNode = testGraph.Values.First();

            var result = singleton.Recognize(testNode);


            Assert.That(result.GetScore(), Is.InRange(minScore, maxScore));
        }
    }
}
