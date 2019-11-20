using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using IDesign.Checks;

namespace IDesign.Regonizers.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("void", @"public void TestMethod(){}", true)]
        [TestCase("int", @"public int TestMethod(){}", true)]
        [TestCase("Class", @"public Class TestMethod(){}", true)]
        [TestCase("Class<T>", @"public Class<T> TestMethod(){}", true)]
        [TestCase("Class<T>", @"public Class<T> TestMethod(){}", true)]
        [TestCase("Class[]", @"public Class[] TestMethod(){}", true)]
        [TestCase("bool", @"public void TestMethod(){}", false)]
        [TestCase("int", @"public bool TestMethod(){}", false)]
        [TestCase("Class", @"public void TestMethod(){}", false)]
        public void ReturnTypeCheck_Returns_CorrectRepsonse(string returnType, string code, bool shouldBeValid)
        {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            var method = root.Members[0] as MethodDeclarationSyntax;

            if (method == null)
                Assert.Fail();

            Assert.AreEqual(method.CheckReturnType(returnType), shouldBeValid);
        }
    }
}