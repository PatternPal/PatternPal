using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using IDesign.Checks;
using IDesign.Tests.Utils;
using IDesign.Core;
using System.Linq;

namespace IDesign.Recognizers.Tests
{
    public class SingletonRecognizerTest
    {
        //[SetUp]
        //public void Setup()
        //{
        //}


        //[Test]
        //[TestCase("SingleTonTestCase1.cs", 100)]
        //[TestCase("SingleTonTestCase2.cs", 100)]
        //[TestCase("SingleTonTestCase3.cs", 100)]
        [TestCase("SingleTonTestCase2.cs", 42)]
        [TestCase("SingleTonTestCase3.cs", 42)]
        //{
        [TestCase("SingleTonTestCase6.cs", 57)]
        [TestCase("SingleTonTestCase7.cs", 57)]
        public void SingletonRecognizer_Returns_Correct_Score(string filename, int score)


        //    var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
        //    var NameSpaceNode = root.Members[0] as NamespaceDeclarationSyntax;
        //    var testNode = NameSpaceNode.Members[0] as ClassDeclarationSyntax;

        //    var entityNode = new EntityNode();
        //    entityNode.Name = testNode.Identifier.ToString();
        //    entityNode.InterfaceOrClassNode = testNode;

        //    entityNode.MethodDeclarationSyntaxList = testNode.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
        //    entityNode.FieldDeclarationSyntaxList = testNode.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();
        //    entityNode.PropertyDeclarationSyntaxList = testNode.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();
            entityNode.ConstructorDeclarationSyntaxList = testNode.DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList();

        //    var result = singleton.Recognize(entityNode);

        //    Assert.AreEqual(score, result.GetScore());

        //}
    }
}
