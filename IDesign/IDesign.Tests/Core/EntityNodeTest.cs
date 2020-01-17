using System.Linq;
using IDesign.Tests.Utils;
using NUnit.Framework;

namespace IDesign.Tests.Core
{
    public class EntityNodeTest
    {
        [Test]
        [TestCase("TestClass1.cs", "x;y")]
        [TestCase("TestClass2.cs", "Getal;Naam;naam;PublicProperty")]
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
        [TestCase("TestClass2.cs", "Count")]
        public void Should_Returns_Correct_Methods(string filename, string expected)
        {
            var code = FileUtils.FileToString(filename);
            var testGraph = EntityNodeUtils.CreateEntityNodeGraphFromOneFile(code);
            var testNode = testGraph.Values.First();
            var fields = string.Join(";", testNode.GetMethodsAndProperties().Select(x => x.GetName()));
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