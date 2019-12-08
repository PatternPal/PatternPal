using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using IDesign.Recognizers;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models;

namespace IDesign.Tests.Checks
{
    public class MethodTest
    {
        [Test]
        [TestCase("void", @"public void TestMethod(){}", true)]
        [TestCase("int", @"public int TestMethod(){}", true)]
        [TestCase("Class", @"public Class TestMethod(){}", true)]
        [TestCase("Class<T>", @"public Class<T> TestMethod(){}", true)]
        [TestCase("Class<T>", @"public Class<T> TestMethod(){}", true)]
        [TestCase("Class[]", @"public Class[] TestMethod(){}", true)]
        [TestCase("bool", @"public void TestMethod(){}", false)]
        [TestCase("int", @"public bool TestMethod(){}", false)]
        [TestCase("Class", @"public void TestMethod(){}", false)]
        public void ReturnTypeCheck_Should_Return_CorrectRepsonse(string returnType, string code, bool shouldBeValid)
        {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var methodSyntax = root.Members[0] as MethodDeclarationSyntax;
            if (methodSyntax == null)
                Assert.Fail();
            var method = new Method(methodSyntax);

            Assert.AreEqual(shouldBeValid, method.CheckReturnType(returnType));
        }

        [Test]
        [TestCase("public", @"public void TestMethod(){}", true)]
        [TestCase("private", @"private int TestMethod(){}", true)]
        [TestCase("static", @"public static Class TestMethod(){}", true)]
        [TestCase("abstract", @"public abstract Class<T> TestMethod(){}", true)]
        [TestCase("private", @"private static Class<T> TestMethod(){}", true)]
        [TestCase("private", @"public Class[] TestMethod(){}", false)]
        [TestCase("public", @"private static void TestMethod(){}", false)]
        public void ModifierCheck_Should_Return_CorrectResponse(string modifier, string code, bool shouldBeValid)
        {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var method = root.Members[0] as MethodDeclarationSyntax;

            if (method == null)
                Assert.Fail();

            Assert.AreEqual(shouldBeValid, new Method(method).CheckModifier(modifier));
        }

        [Test]
        [TestCase(@"public void TestMethod(){new class();}", true)]
        [TestCase(@"public void TestMethod(){string i ='this is a new class';}", false)]
        [TestCase(@"public int TestMethod(){return new int();}", true)]
        [TestCase(@"public void TestMethod(){this.x = new int();}", true)]
        [TestCase(@"public void TestMethod(){string x  = new double().parse();}", true)]
        public void CreationalCheck_Should_Return_CorrectResponse(string code, bool shouldBeVaild)
        {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var method = root.Members[0] as MethodDeclarationSyntax;

            if (method == null)
                Assert.Fail();

            Assert.AreEqual(shouldBeVaild, new Method(method).CheckCreationalFunction());
        }

        [Test]
        [TestCase(@"public void TestMethod(){string i ='this is a new class';}", false)]
        [TestCase(@"public Class TestMethod(){return new Class();}", true)]
        [TestCase(@"public int TestMethod(){return new int();}", false)]
        [TestCase(@"public Class TestMethod(){var x = new Class(); return x;}", true)]
        [TestCase(@"public int TestMethod(){var x = new Class(); return new int();}", false)]
        public void ReturnClassCheck_Should_Return_CorrectResponse(string code, bool shouldBeVaild)
        {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var method = root.Members[0] as MethodDeclarationSyntax;

            if (method == null)
                Assert.Fail();

            Assert.AreEqual(shouldBeVaild, new Method(method).CheckReturnTypeSameAsCreation());
        }
    }
}