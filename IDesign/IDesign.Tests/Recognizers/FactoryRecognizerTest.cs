using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace IDesign.Tests.Recognizers
{
    public class FactoryRecognizerTest
    {
        [SetUp]
        public void Setup()
        {
        }
        
        [Test]
        //[TestCase("FactoryTestCase1.cs", 100)]
        //[TestCase("FactoryTestCase2.cs", 42)]
        //[TestCase("FactoryTestCase3.cs", 42)]
        //[TestCase("FactoryTestCase4.cs", 100)]
        //[TestCase("FactoryTestCase5.cs", 57)]
        //[TestCase("FactoryTestCase6.cs", 57)]
        public void FactoryRecognizer_Returns_Correct_Score(string filename, int score)
        {
            var singleton = new SingletonRecognizer();
            string code = FileUtils.FileToString("FactoryTestClasses\\" + filename);


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
