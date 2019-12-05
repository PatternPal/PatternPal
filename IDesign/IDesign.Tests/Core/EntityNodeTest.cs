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
        [Test]
        [TestCase("TestClass1.cs", "x;y")]
        [TestCase("TestClass2.cs", "Getal;Naam;naam;PublicProperty;_privateField")]
        public void Should_Returns_Correct_Fields(string filename, string expected)
        {
            var code = FileUtils.FileToString(filename);
            var testGraph = EntityNodeUtils.CreateEntityNodeGraphFromOneFile(code);
            var testNode = testGraph.Values.First();
            var fields = string.Join(";", testNode.GetFields().Select(x => x.GetName()));
            Assert.AreEqual(expected, fields);
        }

        [Test]
        [TestCase("TestClass1.cs", "Sum")]
        [TestCase("TestClass2.cs", "Count;PublicProperty")]
        public void Should_Returns_Correct_Methods(string filename, string expected)
        {
            var code = FileUtils.FileToString(filename);
            var testGraph = EntityNodeUtils.CreateEntityNodeGraphFromOneFile(code);
            var testNode = testGraph.Values.First();
            var fields = string.Join(";", testNode.GetMethods().Select(x => x.GetName()));
            Assert.AreEqual(expected, fields);
        }

        [Test]
        [TestCase("TestClass1.cs", 1)]
        [TestCase("TestClass2.cs", 1)]
        public void Should_Returns_Correct_Constructors(string filename, int expected)
        {
            var code = FileUtils.FileToString(filename);
            var testGraph = EntityNodeUtils.CreateEntityNodeGraphFromOneFile(code);
            var testNode = testGraph.Values.First();
            Assert.AreEqual(expected, testNode.GetConstructors().Count());
        }
    }
}