using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.TestClasses.StateTest3;
using IDesign.Tests.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDesign.Tests.Recognizers
{
    class AdapterRecognizerTest
    {
        [TestCase("AdapterTest1", "Target", 0, 79)] //Target
        [TestCase("AdapterTest1", "Adaptee", 0, 79)] //Adaptee
        [TestCase("AdapterTest1", "Adapter", 80, 100)] //Adapter
        [TestCase("AdapterTest2", "ThirdPartyBillingSystem", 0, 80)] //Client
        [TestCase("AdapterTest2", "ITarget", 0, 79)] //Target
        [TestCase("AdapterTest2", "HRSystem", 0, 79)] //Adaptee
        [TestCase("AdapterTest2", "EmployeeAdapter", 80, 100)] //Adapter
        public void AdapterRecognizer_Returns_Correct_Score(string directory, string filename, int minscore, int maxscore)
        {
            var state = new AdapterRecognizer();
            var filesAsString = FileUtils.FilesToString($"{directory}\\");
            var nameSpaceName = $"IDesign.Tests.TestClasses.{directory}";
            var entityNodes = EntityNodeUtils.CreateEntityNodeGraph(filesAsString);
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.GetEdgesOfEntityNode();
            var result = state.Recognize(entityNodes[nameSpaceName + "." + filename]);

            Assert.That(result.GetScore(), Is.InRange(minscore, maxscore));
        }
    }
}
