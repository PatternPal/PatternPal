using IDesign.Tests.Utils;
using System.Collections.Generic;
using IDesign.Recognizers.Checks;
using IDesign.Core;
using NUnit.Framework;

namespace IDesign.Tests.Checks
{
    class EntityNodeChecks
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
            var checkResult = nodes[nameSpaceName + "." + className].ClassImlementsInterface(interfaceName);

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

    }
}
