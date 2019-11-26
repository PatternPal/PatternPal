using IDesign.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDesign.Tests.Core
{
    public class EntityNodeTest
    {
        [SetUp]
        public void Setup()
        {
        }

        public void Should_Create_Fields()
        {
            var testClass = @"";
            var root = CSharpSyntaxTree.ParseText(testClass).GetCompilationUnitRoot();
            var testNode = root.Members[0] as ClassDeclarationSyntax;

            var entityNode = new EntityNode();
            entityNode.Name = testNode.Identifier.ToString();
            entityNode.InterfaceOrClassNode = testNode;
            entityNode.FieldDeclarationSyntaxList = testNode.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();
            entityNode.PropertyDeclarationSyntaxList = testNode.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

            var fields = string.Join(";", entityNode.GetFields().Select(x => x.GetName()));

            

        }
    }
}
