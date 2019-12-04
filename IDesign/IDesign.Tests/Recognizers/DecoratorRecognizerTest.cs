using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System.Linq;

namespace IDesign.Tests.Recognizers
{
    public class DecoratorRecognizerTest
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("DecoratorTestCase1.cs", 100)]
        [TestCase("DecoratorTestCase2.cs", 100)]
        [TestCase("DecoratorTestCase3.cs", 100)]
        [TestCase("DecoratorTestCase4.cs", 50)]
        [TestCase("DecoratorTestCase5.cs", 50)]
        [TestCase("DecoratorTestCase6.cs", 50)]
        [TestCase("DecoratorTestCase7.cs", 0)]
        public void DecoratorRecognizer_Returns_Correct_Score(string filename, int score)
        {
            var decorator = new DecoratorRecognizer();
            string code = FileUtils.FileToString("Decorator\\" + filename);


            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var NameSpaceNode = root.Members[0] as NamespaceDeclarationSyntax;
            var testNode = NameSpaceNode.Members[0] as ClassDeclarationSyntax;

            var entityNode = new EntityNode
            {
                Name = testNode.Identifier.ToString(),
                InterfaceOrClassNode = testNode,

                MethodDeclarationSyntaxList =
                testNode.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList(),
                FieldDeclarationSyntaxList =
                testNode.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList(),
                PropertyDeclarationSyntaxList =
                testNode.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList(),
                ConstructorDeclarationSyntaxList =
                testNode.DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList()
            };

            var result = decorator.Recognize(entityNode);

            Assert.AreEqual(score, result.GetScore());
        }
    }
}
