using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDesign.Tests.Recognizers
{
    public class FactoryRecognizerTest
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("FactorySimpleTestCase1.cs", 100)]
        [TestCase("FactorySimpleTestCase2.cs", 50)]
        [TestCase("FactorySimpleTestCase3.cs", 50)]
        [TestCase("FactorySimpleTestCase4.cs", 100)]
        [TestCase("FactorySimpleTestCase5.cs", 50)]
        public void FactoryRecognizer_Returns_Correct_Score(string filename, int score)
        {
            var factory = new FactoryRecognizer();
            string code = FileUtils.FileToString("FactorySimple\\" + filename);


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

            var result = factory.Recognize(entityNode);

            Assert.AreEqual(score, result.GetScore());
        }
    }
}
