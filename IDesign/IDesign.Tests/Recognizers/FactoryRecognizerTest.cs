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
    public class FactoryRecognizerTest
    {
        [Test]
        [TestCase("FactoryTestCase1", 50)]
        [TestCase("FactoryTestCase2", 100)]
        [TestCase("FactoryTestCase3", 50)]
        [TestCase("FactoryTestCase4", 50)]
        [TestCase("FactoryTestCase5", 50)]
        public void FactoryRecognizer_Returns_Correct_Score(string baseClass, int score)
        {
            var factory = new FactoryRecognizer();
            var filesAsString = FileUtils.FilesToString("Factory\\");
            var nameSpaceName = "IDesign.Tests.TestClasses.Factory";
            var entityNodes = EntityNodeUtils.CreateEntityNodeGraph(filesAsString);
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.GetEdgesOfEntityNode();
            var result = factory.Recognize(entityNodes[nameSpaceName +"." + baseClass]);

            Assert.AreEqual(score, result.GetScore());
        }
    }
}
