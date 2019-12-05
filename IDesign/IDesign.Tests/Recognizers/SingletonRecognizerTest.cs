using System.Linq;
using IDesign.Core;
using IDesign.Tests.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace IDesign.Recognizers.Tests
{
    public class SingletonRecognizerTest
    {
        [Test]
        [TestCase("SingleTonTestCase1.cs", 100)]
        [TestCase("SingleTonTestCase2.cs", 42)]
        [TestCase("SingleTonTestCase3.cs", 42)]
        [TestCase("SingleTonTestCase4.cs", 100)]
        [TestCase("SingleTonTestCase6.cs", 100)]
        [TestCase("SingleTonTestCase7.cs", 57)]
        public void SingletonRecognizer_Returns_Correct_Score(string filename, int score)
        {
            var singleton = new SingletonRecognizer();
            string code = FileUtils.FileToString("SingletonTestClasses\\" + filename);


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

            var result = singleton.Recognize(entityNode);

            Assert.AreEqual(score, result.GetScore());
        }
    }
}
