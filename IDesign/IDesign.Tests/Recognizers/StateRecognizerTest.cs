using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.TestClasses.StateTest3;
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
 
    class StateRecognizerTest
    {
        //[TestCase("ConcreteStateA.cs", 100)]
        //[TestCase("ConcreteStateB.cs", 100)]
        //[TestCase("Context.cs", 100)]
        [TestCase("State.cs", 100)]
        public void StateRecognizer_Returns_Correct_Score(string filename, int score)
        {
            var singleton = new StateRecognizer();
            string code = FileUtils.FileToString("StateTest3\\" + filename);


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
