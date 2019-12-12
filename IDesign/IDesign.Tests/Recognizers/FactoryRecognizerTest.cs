using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using NUnit.Framework;

namespace IDesign.Tests.Recognizers
{
    public class FactoryRecognizerTest
    {
        [Test]
        [TestCase("FactoryTestCase1", 66)]
        [TestCase("FactoryTestCase2", 33)]
        [TestCase("FactoryTestCase3", 33)]
        [TestCase("FactoryTestCase4", 66)]
        [TestCase("FactoryTestCase5", 100)]
        [TestCase("FactoryTestCase6", 33)]
        [TestCase("FactoryTestCase7", 100)]
        [TestCase("FactoryTestCase8", 66)]
        public void FactoryRecognizer_Returns_Correct_Score(string baseClass, int score)
        {
            var factory = new FactoryRecognizer();
            var filesAsString = FileUtils.FilesToString("Factory\\");
            var nameSpaceName = "IDesign.Tests.TestClasses.Factory";
            var entityNodes = EntityNodeUtils.CreateEntityNodeGraph(filesAsString);
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.GetEdgesOfEntityNode();
            var result = factory.Recognize(entityNodes[nameSpaceName + "." + baseClass]);

            Assert.AreEqual(score, result.GetScore());
        }
    }
}