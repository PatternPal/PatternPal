using System.Collections.Generic;
using System.Linq;
using IDesign.Core;
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

      

        [TestCase("TestClass1.cs","TestClass1")]
        [TestCase("TestClass2.cs","FirstTestClass")]
        [TestCase("TestClass2.cs","IFirstTestClass")]
        public void Should_Return_Name(string filename, string expected)
        {
            EntityNode entityNode = new EntityNode();
            var content = FileUtils.FileToString(filename);

            var Tree = CSharpSyntaxTree.ParseText(content);
            var Root = Tree.GetCompilationUnitRoot();

            var members = Root.Members;

            string result = "";
            if(members != null)
            {
                foreach(var member in members)
                {
                    if(member.Kind() == SyntaxKind.ClassDeclaration)
                    {
                        var className = (ClassDeclarationSyntax)member;
                        result = className.Identifier.ToString();
                        Assert.AreEqual(expected, result);

                    }
                }
            }




        }
    }
}