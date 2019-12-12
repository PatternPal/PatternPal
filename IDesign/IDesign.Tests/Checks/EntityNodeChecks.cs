using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using IDesign.Recognizers;
using IDesign.Recognizers.Checks;
using IDesign.Core;
using System.Collections.Generic;
using IDesign.Tests.Utils;
using System;

namespace IDesign.Tests.Checks
{
    class EntityNodeChecks
    {
        [Test]
        [TestCase("Class1", "int", 2, true)]
        [TestCase("Class1", "int", 3, false)]
        [TestCase("Class1", "string", 1, true)]
        [TestCase("Class2", "string", 2, true)]
        [TestCase("Class2", "string", 3, true)]
        [TestCase("Class2", "string", 4, false)]
        [TestCase("Class2", "int", 1, false)]
        public void CheckMinimalAmountOfMethodsWithParameter_Should_Return_Correct_Response(string className, string parameter, int amount, bool shouldBeValid)
        {
            var code = FileUtils.FileToString("../../../../TestClasses/EntityNodeChecks/" + className + ".cs");
         var nameSpaceName = "IDesign.Tests.TestClasses.EntityNodeChecks";
            var nodes = EntityNodeUtils.CreateEntityNodeGraphFromOneFile(code);
            var createRelation = new DetermineRelations(nodes);
            createRelation.GetEdgesOfEntityNode();
            var checkResult = nodes[nameSpaceName + "." + className].CheckMinimalAmountOfMethodsWithParameter(new List<string> { parameter }, amount);

            Assert.AreEqual(shouldBeValid, checkResult);
        }

        [Test]
        [TestCase("Class1", 1, true)]
        [TestCase("Class1", 2, true)]
        [TestCase("Class1", 3, true)]
        [TestCase("Class1", 4, false)]
        [TestCase("Class3", 0, true)]
        [TestCase("Class3", 1, false)]
        public void CheckMinimalAmountOfMethods_Should_Return_Correct_Response(string className, int amount, bool shouldBeValid)
        {
            var code = FileUtils.FileToString("../../../../TestClasses/EntityNodeChecks/" + className + ".cs");
            var nameSpaceName = "IDesign.Tests.TestClasses.EntityNodeChecks";
            var nodes = EntityNodeUtils.CreateEntityNodeGraphFromOneFile(code);
            var createRelation = new DetermineRelations(nodes);
            createRelation.GetEdgesOfEntityNode();
            var checkResult = nodes[nameSpaceName + "." + className].CheckMinimalAmountOfMethods(amount);

            Assert.AreEqual(shouldBeValid, checkResult);
        }
    }
}
