using IDesign.Recognizers.Checks;
using IDesign.Tests.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using SyntaxTree;
using SyntaxTree.Abstractions;

namespace IDesign.Tests.Checks {
    class EntityNodeChecksTest {
        [TestCase("Abstract", @"abstract class TestClass1 { }", true)]
        [TestCase("Abstract", @"class TestClass1 { }", false)]
        [TestCase("Public", @"public class TestClass1 { }", true)]
        [TestCase("public", @"class TestClass1 { }", false)]
        [TestCase("Private", @"private class TestClass1 { }", true)]
        [TestCase("private", @"class TestClass1 { }", false)]
        [TestCase("Internal", @"internal class TestClass1 { }", true)]
        [TestCase("Internal", @"class TestClass1 { }", false)]
        public void ModifierCheck_Should_Return_CorrectRepsonse(string modifier, string code, bool shouldBeValid) {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var typeDeclarationSyntax = root.Members[0] as TypeDeclarationSyntax;
            if (typeDeclarationSyntax == null) {
                Assert.Fail();
            }

            var entity = EntityNodeUtils.CreateTestEntityNode(typeDeclarationSyntax);

            Assert.AreEqual(shouldBeValid, entity.CheckModifier(modifier));
        }

        [TestCase("Decorator", RelationType.Implements, 1, true)]
        [TestCase("Decorator", RelationType.Implements, 2, false)]
        [TestCase("Decorator", RelationType.ExtendedBy, 1, true)]
        [TestCase("Decorator", RelationType.ExtendedBy, 2, true)]
        [TestCase("Decorator", RelationType.ExtendedBy, 3, false)]
        [TestCase("Decorator", RelationType.Extends, 1, false)]
        [TestCase("Decorator", RelationType.ImplementedBy, 1, false)]
        [TestCase("IComponent", RelationType.Implements, 1, false)]
        [TestCase("IComponent", RelationType.ImplementedBy, 1, true)]
        [TestCase("IComponent", RelationType.ImplementedBy, 2, true)]
        public void MinimalAmountOfRelationTypesCheck_Should_Return_Correct_RelationType(
            string className,
            RelationType relation,
            int amount,
            bool shouldBeValid
        ) {
            var filesAsString = FileUtils.FilesToString("../../../../TestClasses/Decorator/DecoratorTestCase1");
            var nameSpace = "IDesign.Tests.TestClasses.Decorator.DecoratorTestCase1";
            var graph = new SyntaxGraph();
            var i = 0;
            foreach (var content in filesAsString) {
                graph.AddFile(content, i++.ToString());
            }

            graph.CreateGraph();
            var nodes = graph.GetAll();

            Assert.AreEqual(shouldBeValid, nodes[nameSpace].CheckMinimalAmountOfRelationTypes(relation, amount));
        }

        [TestCase("Class1", "IClass1", true)]
        [TestCase("Class2", "IClass2", true)]
        [TestCase("Class3", "IClass3", false)]
        [TestCase("Class4", "IClass4", true)]
        [TestCase("Class5", "IClass5", true)]
        public void ClassImplementsInterface(string className, string interfaceName, bool shouldBeValid) {
            var code = FileUtils.FilesToString("ClassChecks\\");
            var nameSpaceName = "IDesign.Tests.TestClasses.ClassChecks";
            var graph = new SyntaxGraph();
            var i = 0;
            foreach (var content in code) {
                graph.AddFile(content, i++.ToString());
            }

            graph.CreateGraph();
            var nodes = graph.GetAll();

            var checkResult = nodes[nameSpaceName + "." + className].ImplementsInterface(interfaceName);

            Assert.AreEqual(shouldBeValid, checkResult);
        }

        [TestCase("Class1", "EClass1", true)]
        [TestCase("Class2", "EClass2", true)]
        [TestCase("Class3", "EClass3", true)]
        [TestCase("Class4", "EClass4", true)]
        [TestCase("Class5", "E1Class5", true)]
        public void ClassExtendsClass(string className, string eClassName, bool shouldBeValid) {
            var code = FileUtils.FilesToString("ClassChecks\\");
            var nameSpaceName = "IDesign.Tests.TestClasses.ClassChecks";
            var graph = new SyntaxGraph();
            var i = 0;
            foreach (var content in code) {
                graph.AddFile(content, i++.ToString());
            }

            graph.CreateGraph();
            var nodes = graph.GetAll();

            var checkResult = nodes[nameSpaceName + "." + className].ExtendsClass(eClassName);

            Assert.AreEqual(shouldBeValid, checkResult);
        }

        [TestCase("Class1", "IClass1", true)]
        [TestCase("Class2", "IClass2", false)]
        [TestCase("Class3", "IClass3", false)]
        [TestCase("Class4", "IClass4", false)]
        [TestCase("Class5", "IClass5", false)]
        public void ClassImplementsInterfaceDirectly(string className, string interfaceName, bool shouldBeValid) {
            var code = FileUtils.FilesToString("ClassChecks\\");
            var nameSpaceName = "IDesign.Tests.TestClasses.ClassChecks";
            var graph = new SyntaxGraph();
            var i = 0;
            foreach (var content in code) {
                graph.AddFile(content, i++.ToString());
            }

            graph.CreateGraph();
            var nodes = graph.GetAll();
            var checkResult = nodes[nameSpaceName + "." + className].ClassImplementsInterface(interfaceName);

            Assert.AreEqual(shouldBeValid, checkResult);
        }
    }
}
