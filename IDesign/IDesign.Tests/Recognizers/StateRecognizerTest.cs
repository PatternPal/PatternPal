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
    class StateRecognizerTest
    {
        [TestCase("StateTest1","Doneness", 80,100)]
        [TestCase("StateTest2", "State", 80,100)]
        [TestCase("StateTest3", "State", 80,100)]
        [TestCase("StateTest4", "IState", 0,79)]
        [TestCase("StateFactoryMethodTest1", "IState", 80,100)]
        public void StateRecognizer_Returns_Correct_Score(string directory, string filename, int minScore, int maxScore)
        {
            var state = new StateRecognizer();
            var filesAsString = FileUtils.FilesToString($"{directory}\\");
            var nameSpaceName = $"IDesign.Tests.TestClasses.{directory}";
            var entityNodes = EntityNodeUtils.CreateEntityNodeGraph(filesAsString);
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.GetEdgesOfEntityNode();
            var result = state.Recognize(entityNodes[nameSpaceName + "." + filename]);

            Assert.That(result.GetScore(), Is.InRange(minScore, maxScore));
        }
    }
}
