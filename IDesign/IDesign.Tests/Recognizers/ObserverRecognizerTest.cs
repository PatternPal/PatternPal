using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace IDesign.Tests.Recognizers
{
    public class ObserverRecognizerTest
    {
        [Test]
        [TestCase("Case1", 100)]
        [TestCase("Case2", 85)]
        [TestCase("Case3", 85)]
        [TestCase("Case4", 71)]
        [TestCase("Case5", 71)]
        public void ObserverRecognizer_Returns_Correct_Score(string directoryPath, int score)
        {
            var recognizer = new ObserverRecognizer();
            var manager = new FileManager();
            var paths = manager.GetAllCsFilesFromDirectory("../../../TestClasses/Observer/" + directoryPath);
            var runner = new RecognizerRunner();

            List<RecognitionResult> result = runner.Run(paths, new List<DesignPattern>() { new DesignPattern("Observer", new ObserverRecognizer()) });

            result = result.OrderBy(x => x.Result.GetScore()).ToList();

            Assert.AreEqual(score, result.Last().Result.GetScore());
        }
    }
}
