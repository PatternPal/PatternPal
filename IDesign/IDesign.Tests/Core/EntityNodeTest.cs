using System.Collections.Generic;
using System.Linq;
using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace IDesign.Tests.Core
{
    public class EntityNodeTest
    {
        public void Should_Create_Fields()
        {
            var testClass = @"";
            var root = CSharpSyntaxTree.ParseText(testClass).GetCompilationUnitRoot();
            var testNode = root.Members[0] as ClassDeclarationSyntax;

            var entityNode = new EntityNode();
            entityNode.Name = testNode.Identifier.ToString();
            entityNode.InterfaceOrClassNode = testNode;
            entityNode.FieldDeclarationSyntaxList =
                testNode.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();
            entityNode.PropertyDeclarationSyntaxList =
                testNode.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

            var fields = string.Join(";", entityNode.GetFields().Select(x => x.GetName()));
        }


        RecognizerRunner recognizerRunner = new RecognizerRunner();
        List<DesignPattern> designPatterns = new List<DesignPattern> { new DesignPattern("Singleton", new SingletonRecognizer()),
            new DesignPattern("Factory Method", new FactoryRecognizer())};

        private readonly Dictionary<TypeDeclarationSyntax, EntityNode> entityNodes =
            new Dictionary<TypeDeclarationSyntax, EntityNode>();

        [TestCase(@"using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses
{
    class TestClass1
    {
        public TestClass1(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x { get; set; }
        public int y { get; set; }

        public int Sum()
        {
            return x + y;
        }
    }
}", new string[] { "TestClass1" })]
        [TestCase("TestClass2.cs", new string[] { "FirstTestClass", "IFirstTestClass" })]
        public void Should_Return_Name(string file, string[] expected)
        {

            GenerateSyntaxTree generateSyntaxTree = new GenerateSyntaxTree(file, "", entityNodes);
            string[] results = new string[256];
            //for (int i = 0; i < entityNodes.Count; i++)
            //{
            //    for(int j = 0; j < entityNodes.Count; j++)
            //    {
            //        var name = entityNodes[];
            //    }
            //    results[i] = name;
            //    System.Console.WriteLine(result);
            //    Assert.AreEqual(expected, result);
            //}

        }

   

    }
}