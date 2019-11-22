using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using IDesign.Checks;
using IDesign.Core;
using System.Linq;

namespace IDesign.Recognizers.Tests
{
    public class SingletonRecognizerTest
    {
        [SetUp]
        public void Setup()
        {
        }




        [Test]
        public void TestFunction()
        {
            var singleton = new SingletonRecognizer();

            var testClass = @"public sealed class Singleton

    {
    private static Singleton instance = null;

    private Singleton()
    {
    }

    public static Singleton GetInstance()
    {
    if (instance==null)
    {
    instance = new Singleton();
    }
    return instance;
    }
    }
        
";
            var root = CSharpSyntaxTree.ParseText(testClass).GetCompilationUnitRoot();
            var testNode = root.Members[0] as ClassDeclarationSyntax;

            var entityNode = new EntityNode();
            entityNode.Name = testNode.Identifier.ToString();
            entityNode.InterfaceOrClassNode = testNode;

            entityNode.MethodDeclarationSyntaxList = testNode.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            entityNode.FieldDeclarationSyntaxList = testNode.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();
            entityNode.PropertyDeclarationSyntaxList = testNode.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

            var result = singleton.Recognize(entityNode);

            Assert.AreEqual(100, result.GetScore());

        }
    }
}