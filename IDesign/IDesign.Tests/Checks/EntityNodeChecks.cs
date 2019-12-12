using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Recognizers.Checks;
using IDesign.Tests.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System.Linq;

namespace IDesign.Tests.Checks
{
    class EntityNodeChecks
    {
        [Test]
        [TestCase("Abstract", @"abstract class TestClass1 { }", true)]
        [TestCase("Abstract", @"class TestClass1 { }", false)]
        [TestCase("Public", @"public class TestClass1 { }", true)]
        [TestCase("public", @"class TestClass1 { }", false)]
        [TestCase("Private", @"private class TestClass1 { }", true)]
        [TestCase("private", @"class TestClass1 { }", false)]
        [TestCase("Internal", @"internal class TestClass1 { }", true)]
        [TestCase("Internal", @"class TestClass1 { }", false)]
        public void ModifierCheck_Should_Return_CorrectRepsonse(string modifier, string code, bool shouldBeValid)
        {

            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var typeDeclarationSyntax = root.Members[0] as TypeDeclarationSyntax;
            if (typeDeclarationSyntax == null)
                Assert.Fail();

            var entityNode = new EntityNode
            {
                InterfaceOrClassNode = typeDeclarationSyntax
            };

            Assert.AreEqual(shouldBeValid, entityNode.CheckModifier(modifier));
        }

        [Test]
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
        public void MinimalAmountOfRelationTypesCheck_Should_Return_Correct_RelationType(string className, RelationType relation, int amount, bool shouldBeValid)
        {
            var filesAsString = FileUtils.FilesToString("../../../../TestClasses/Decorator/DecoratorTestCase1");
            var nameSpace = "IDesign.Tests.TestClasses.Decorator.DecoratorTestCase1";
            var entityNodes = EntityNodeUtils.CreateEntityNodeGraph(filesAsString);
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.GetEdgesOfEntityNode();
            var entityNode = entityNodes[nameSpace + "." + className];

            Assert.AreEqual(shouldBeValid, entityNode.CheckMinimalAmountOfRelationTypes(relation, amount));   
        }
    }
}
