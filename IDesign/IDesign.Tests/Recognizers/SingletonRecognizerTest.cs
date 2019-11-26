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
        //[TestCase("SingleTonTestCase4.cs", 100)]
        //public void TestFunction(string filename, int score)
        //{
        //    var singleton = new SingletonRecognizer();
        //    string code = FileUtils.FileToString(filename);


        //    var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
        //    var NameSpaceNode = root.Members[0] as NamespaceDeclarationSyntax;
        //    var testNode = NameSpaceNode.Members[0] as ClassDeclarationSyntax;

        //    var entityNode = new EntityNode();
        //    entityNode.Name = testNode.Identifier.ToString();
        //    entityNode.InterfaceOrClassNode = testNode;

        //    entityNode.MethodDeclarationSyntaxList = testNode.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
        //    entityNode.FieldDeclarationSyntaxList = testNode.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();
        //    entityNode.PropertyDeclarationSyntaxList = testNode.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

        //    var result = singleton.Recognize(entityNode);

        //    Assert.AreEqual(score, result.GetScore());

        //}
    }
}
