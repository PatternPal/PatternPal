using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace IDesign.Tests.Recognizers
{
    public class DecoratorRecognizerTest
    {
        [Test]
        [TestCase("DecoratorTestCase1", 100)]
        [TestCase("DecoratorTestCase2", 0)]
        [TestCase("DecoratorTestCase3", 13)]
        [TestCase("DecoratorTestCase4", 40)]
        [TestCase("DecoratorTestCase5", 53)]
        public void DecoratorRecognizer_Returns_Correct_Score(string directoryPath, int score)
        {
            FileManager manager = new FileManager();
            List<string> paths = manager.GetAllCsFilesFromDirectory("../../../TestClasses/Decorator/" + directoryPath);

            RecognizerRunner runner = new RecognizerRunner();
            List<RecognitionResult> result = runner.Run(paths, new List<DesignPattern>() { new DesignPattern("Decorator", new DecoratorRecognizer()) });

            result = result.OrderBy(x => x.Result.GetScore()).ToList();

            Assert.AreEqual(score, result.Last().Result.GetScore());
        }
    }
}
