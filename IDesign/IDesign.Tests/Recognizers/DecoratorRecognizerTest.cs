using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System.Collections.Generic;
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
        [TestCase("D:/Desktop/decorator-pattern/DecoratorPattern/DecoratorPattern", 100)]
        public void DecoratorRecognizer_Returns_Correct_Score(string directoryPath, int score)
        {
            FileManager manager = new FileManager();
            List<string> paths = manager.GetAllCsFilesFromDirectory(directoryPath);

            RecognizerRunner runner = new RecognizerRunner();
            List<RecognitionResult> result = runner.Run(paths, new List<DesignPattern>() { new DesignPattern("Decorator", new DecoratorRecognizer()) });

            result = result.OrderBy(x => x.Result.GetScore()).ToList();

            Assert.AreEqual(score, result.Last().Result.GetScore());
        }


        /*[TestCase("DecoratorTestCase1.cs", 100)]
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
        }*/
    }
}
