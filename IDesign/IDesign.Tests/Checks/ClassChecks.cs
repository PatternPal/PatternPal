using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using System.Collections.Generic;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Core;

namespace IDesign.Tests.Checks
{
    public class ClassChecks
    {

        [Test]
        [TestCase("Class1", "IClass1", true)]
        [TestCase("Class2", "IClass2", true)]
        [TestCase("Class3", "IClass3", false)]
        [TestCase("Class4", "IClass4", true)]
        [TestCase("Class5", "IClass5", true)]
        public void CLassImplementsInterface(string className, string interfaceName, bool shouldBeValid)
        {
            var code = FileUtils.FilesToString("ClassChecks\\");
            var nameSpaceName = "IDesign.Tests.TestClasses.ClassChecks";
            var nodes = EntityNodeUtils.CreateEntityNodeGraph(code);
            var createRelation = new DetermineRelations(nodes);
            createRelation.GetEdgesOfEntityNode();
            var checkResult = nodes[nameSpaceName + "." + className].ImplementsInterface(interfaceName);


            Assert.AreEqual(shouldBeValid, checkResult);
        }

        [Test]
        [TestCase("Class1", "EClass1", true)]
        [TestCase("Class2", "EClass2", true)]
        [TestCase("Class3", "EClass3", true)]
        [TestCase("Class4", "EClass4", true)]
        [TestCase("Class5", "E1Class5", true)]
        public void ClassExtendsClass(string className, string eClassName, bool shouldBeValid)
        {
            var code = FileUtils.FilesToString("ClassChecks\\");
            var nameSpaceName = "IDesign.Tests.TestClasses.ClassChecks";
            var nodes = EntityNodeUtils.CreateEntityNodeGraph(code);
            var createRelation = new DetermineRelations(nodes);
            createRelation.GetEdgesOfEntityNode();
            var checkResult = nodes[nameSpaceName + "." + className].ExtendsClass(eClassName);


            Assert.AreEqual(shouldBeValid, checkResult);
        }

        [Test]
        [TestCase("Class1", "IClass1", true)]
        [TestCase("Class2", "IClass2", false)]
        [TestCase("Class3", "IClass3", false)]
        [TestCase("Class4", "IClass4", false)]
        [TestCase("Class5", "IClass5", false)]
        public void CLassImplementsInterfaceDirectly(string className, string interfaceName, bool shouldBeValid)
        {
            var code = FileUtils.FilesToString("ClassChecks\\");
            var nameSpaceName = "IDesign.Tests.TestClasses.ClassChecks";
            var nodes = EntityNodeUtils.CreateEntityNodeGraph(code);
            var createRelation = new DetermineRelations(nodes);
            createRelation.GetEdgesOfEntityNode();
            var checkResult = nodes[nameSpaceName + "." + className].HasInterface(interfaceName);


            Assert.AreEqual(shouldBeValid, checkResult);
        }
    }
}
