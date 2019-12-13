using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using NUnit.Framework;

namespace IDesign.Tests.Recognizers
{
    public class FactoryMethodRecognizerTest
    {
        [TestCase("FactoryMethodTest1", "BeerFactory", 100)]
        [TestCase("FactoryMethodTest2", "CardFactory", 100)]
        public void FactoryMethodRecognizer_Returns_Correct_Score(string directory, string filename, int score)
        {
            var factoryMethodRecognizer = new FactoryMethodRecognizer();
            var filesAsString = FileUtils.FilesToString($"{directory}\\");
            var nameSpaceName = $"IDesign.Tests.TestClasses.{directory}";
            var entityNodes = EntityNodeUtils.CreateEntityNodeGraph(filesAsString);
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.GetEdgesOfEntityNode();
            var result = factoryMethodRecognizer.Recognize(entityNodes[nameSpaceName + "." + filename]);

            Assert.AreEqual(score, result.GetScore());
        }
    }
}