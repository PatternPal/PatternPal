﻿using System;
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
        [Test]
        [TestCase("FactorySimpleTestCase1.cs", 100)]
        [TestCase("FactorySimpleTestCase2.cs", 50)]
        [TestCase("FactorySimpleTestCase3.cs", 50)]
        [TestCase("FactorySimpleTestCase4.cs", 100)]
        [TestCase("FactorySimpleTestCase5.cs", 50)]
        public void FactoryRecognizer_Returns_Correct_Score(string filename, int score)
        {
            var factory = new FactoryRecognizer();
            var code = FileUtils.FileToString("FactorySimple\\" + filename);

            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var nameSpaceNode = root.Members[0] as NamespaceDeclarationSyntax;
            foreach (var testNode in nameSpaceNode.Members.OfType<TypeDeclarationSyntax>())
            {

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

            }
        }
    }
}
