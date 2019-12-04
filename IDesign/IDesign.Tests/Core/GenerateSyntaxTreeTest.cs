using System.Collections.Generic;
using IDesign.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace IDesign.Tests.Core
{
    internal class GenerateSyntaxTreeTest
    {
        private readonly Dictionary<TypeDeclarationSyntax, EntityNode> entityNodes =
            new Dictionary<TypeDeclarationSyntax, EntityNode>();

        [TestCase(
            @"namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;}}}",
            1)]
        [TestCase(
            @"namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;} class InnerClass{}}",
            2)]
        [TestCase(
            @"namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;} class InnerClass{}}}",
            2)]
        [TestCase(
            @"namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;}} class TestClass2{}}",
            2)]
        [TestCase(
            @"namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;} class InnerClass{}} class TestClass2{}} class OuterClass{}",
            4)]
        [TestCase(
            @"namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;} class InnerClass{ class ClassInInnerClass{}}} class TestClass2{}} class OuterClass{}",
            5)]
        [TestCase(
            @"namespace TestNamespace{ interface ITestClass{}class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;}}}",
            1)]
        [TestCase(
            @"namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;} interface ITestClass{}}}",
            1)]
        [TestCase(
            @"namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;}}} interface ITestClass{}",
            1)]
        public void TestAmountOfClasses(string file, int expectedAmountOfClasses)
        {
            var generateSyntaxTree = new GenerateSyntaxTree(file, "", entityNodes);
            var result = generateSyntaxTree.ClassDeclarationSyntaxList.Count;
            Assert.AreEqual(expectedAmountOfClasses, result);
        }

        [TestCase(
            @"namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;}}}",
            0)]
        [TestCase(
            @"namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;} class InnerClass{}}",
            0)]
        [TestCase(
            @"namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;} class InnerClass{}}}",
            0)]
        [TestCase(
            @"namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;}} class TestClass2{}}",
            0)]
        [TestCase(
            @"namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;} class InnerClass{}} class TestClass2{}} class OuterClass{}",
            0)]
        [TestCase(
            @"namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;} class InnerClass{ class ClassInInnerClass{}}} class TestClass2{}} class OuterClass{}",
            0)]
        [TestCase(
            @"namespace TestNamespace{ interface ITestClass{}class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;}}}",
            1)]
        [TestCase(
            @"namespace TestNamespace{ interface ITest{}class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;} interface ITestClass{}}}",
            2)]
        [TestCase(
            @"namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;}}} interface ITestClass{interface ITest{}}",
            2)]
        public void TestAmountOfInterfaces(string file, int expectedAmountOfInterfaces)
        {
            var generateSyntaxTree = new GenerateSyntaxTree(file, "", entityNodes);
            var result = generateSyntaxTree.InterfaceDeclarationSyntaxList.Count;
            Assert.AreEqual(expectedAmountOfInterfaces, result);
        }


        [TestCase(@"using IDesign.Core;
                    using NUnit.Framework;
                    using System;
                    using System.Collections.Generic;
                    using System.Text;
                    namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;}}}",
            5)]
        [TestCase(@"using IDesign.Core;
                    using NUnit.Framework;
                    namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;}}}",
            2)]
        [TestCase(@"using System;
                    using System.Collections.Generic;
                    using System.Text;
                    using IDesign.Core;
                    using Microsoft.CodeAnalysis.CSharp;
                    using Microsoft.CodeAnalysis.CSharp.Syntax;
                    using Microsoft.CodeAnalysis.Text;
                    using NUnit.Framework;
                    namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;}}}",
            8)]
        [TestCase(@"using System;
                    using System.ComponentModel.Design;
                    using System.Threading.Tasks;
                    using Microsoft.VisualStudio.Shell;
                    using Microsoft.VisualStudio.Shell.Interop;
                    using Task = System.Threading.Tasks.Task;
                    namespace TestNamespace{ class TestClass{ public string Name {get; set;} public TestClass(string name){this.Name = name;}}}",
            6)]
        public void TestIfAmountOfUsingsIsRight(string file, int amountOfUsings)
        {
            var generateSyntaxTree = new GenerateSyntaxTree(file, "", entityNodes);
            var result = generateSyntaxTree.UsingDirectiveSyntaxList;
            var count = 0;

            foreach (var i in result)
                if (i.Kind() == SyntaxKind.UsingDirective)
                    count++;
            Assert.AreEqual(amountOfUsings, count);
        }
    }
}