using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDesign.Core;
using IDesign.Core.Models;
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
        [TestCase("Case2", 100)]
        [TestCase("Case3", 90)]
        [TestCase("Case4", 63)]
        [TestCase("Case5", 70)]
        public void ObserverRecognizer_Returns_Correct_Score(string directoryPath, int score)
        {
            var recognizer = new ObserverRecognizer();
            var manager = new FileManager();
            var paths = manager.GetAllCsFilesFromDirectory("../../../TestClasses/Observer/" + directoryPath);
            var runner = new RecognizerRunner();

            runner.CreateGraph(paths);
            List<RecognitionResult> result = runner.Run(new List<DesignPattern>() { new DesignPattern("Observer", new ObserverRecognizer(), "") });

            result = result.OrderBy(x => x.Result.GetScore()).ToList();

            Assert.AreEqual(score, result.Last().Result.GetScore());
        }
    }
}
