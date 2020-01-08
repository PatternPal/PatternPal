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
        [TestCase("ObserverTest1", "IObserver", 80, 100)]
        [TestCase("ObserverTest2", "IInvestor", 80, 100)]
        [TestCase("ObserverTest3", "IObserver", 80, 100)]
        [TestCase("ObserverTest4", "IObserver", 0, 79)]
        [TestCase("ObserverTest5", "IObserver", 0, 79)]
        public void ObserverRecognizer_Returns_Correct_Score(string directory, string filename, int minScore, int maxScore)
        {
            var observer = new ObserverRecognizer();
            var filesAsString = FileUtils.FilesToString($"{directory}\\");
            var nameSpaceName = $"IDesign.Tests.TestClasses.{directory}";
            var entityNodes = EntityNodeUtils.CreateEntityNodeGraph(filesAsString);
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.GetEdgesOfEntityNode();
            var result = observer.Recognize(entityNodes[nameSpaceName + "." + filename]);

            Assert.That(result.GetScore(), Is.InRange(minScore, maxScore));
        }
    }
}
