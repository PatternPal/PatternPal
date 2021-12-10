using System.Collections.Generic;
using IDesign.Recognizers.Checks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using SyntaxTree.Abstractions.Members;
using SyntaxTree.Models.Members.Method;

namespace IDesign.Tests.Checks {
    public class MethodTest {
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
        public void ReturnTypeCheck_Should_Return_CorrectRepsonse(string returnType, string code, bool shouldBeValid) {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var methodSyntax = root.Members[0] as MethodDeclarationSyntax;
            if (methodSyntax == null) {
                Assert.Fail();
            }

            var method = new Method(methodSyntax, null);

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
        public void ModifierCheck_Should_Return_CorrectResponse(string modifier, string code, bool shouldBeValid) {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var method = root.Members[0] as MethodDeclarationSyntax;

            if (method == null) {
                Assert.Fail();
            }

            Assert.AreEqual(shouldBeValid, new Method(method, null).CheckModifier(modifier));
        }

        [Test]
        [TestCase(@"public void TestMethod(){new class();}", true)]
        [TestCase(@"public void TestMethod(){string i ='this is a new class';}", false)]
        [TestCase(@"public int TestMethod(){return new int();}", true)]
        [TestCase(@"public void TestMethod(){this.x = new int();}", true)]
        [TestCase(@"public void TestMethod(){string x  = new double().parse();}", true)]
        public void CreationalCheck_Should_Return_CorrectResponse(string code, bool shouldBeVaild) {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var method = root.Members[0] as MethodDeclarationSyntax;

            if (method == null) {
                Assert.Fail();
            }

            Assert.AreEqual(shouldBeVaild, new Method(method, null).CheckCreationalFunction());
        }

        [Test]
        [TestCase(@"public void TestMethod(){string i ='this is a new class';}", false)]
        [TestCase(@"public Class TestMethod(){return new Class();}", true)]
        [TestCase(@"public int TestMethod(){return new int();}", false)]
        [TestCase(@"public Class TestMethod(){var x = new Class(); return x;}", true)]
        [TestCase(@"public int TestMethod(){var x = new Class(); return new int();}", false)]
        public void ReturnClassCheck_Should_Return_CorrectResponse(string code, bool shouldBeVaild) {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var method = root.Members[0] as MethodDeclarationSyntax;

            if (method == null) {
                Assert.Fail();
            }

            Assert.AreEqual(shouldBeVaild, new Method(method, null).CheckReturnTypeSameAsCreation());
        }

        [Test]
        [TestCase(@"public void TestMethod(string s){ }", true, "string", "int")]
        [TestCase(@"public void TestMethod(string s, int i){ }", true, "string", "int")]
        [TestCase(@"public void TestMethod(int i){ }", true, "string", "int")]
        [TestCase(@"public void TestMethod(){ }", false, "string", "int")]
        [TestCase(@"public void TestMethod(IComponent1 comp1){ }", true, "IComponent1")]
        public void ParameterCheck_Should_Return_CorrectResponse(
            string code,
            bool shouldBeVaild,
            params string[] parameters
        ) {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var method = root.Members[0] as MethodDeclarationSyntax;

            if (method == null) {
                Assert.Fail();
            }

            Assert.AreEqual(shouldBeVaild, new Method(method, null).CheckParameters(parameters));
        }

        [Test]
        [TestCase(@"public void TestMethod(){ }", true, @"public void TestMethod(){ }")]
        [TestCase(
            @"public void TestMethod(){ }", true, @"public void TestMethod(){ }", @"public void TestMethod1(){ }"
        )]
        [TestCase(
            @"public void TestMethod(){ }", false, @"public void TestMethod2(){ }", @"public void TestMethod1(){ }"
        )]
        public void NameCheck_Should_Return_CorrectResponse(
            string code,
            bool shouldBeVaild,
            params string[] methodStrings
        ) {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var method = root.Members[0] as MethodDeclarationSyntax;
            List<IMethod> methods = new List<IMethod>();
            foreach (var methodString in methodStrings) {
                var root2 = CSharpSyntaxTree.ParseText(methodString).GetCompilationUnitRoot();
                methods.Add(new Method(root2.Members[0] as MethodDeclarationSyntax, null));
            }

            if (method == null) {
                Assert.Fail();
            }

            Assert.AreEqual(shouldBeVaild, new Method(method, null).CheckIfNameExists(methods));
        }

        [Test]
        [TestCase(@"public void TestMethod(Test test) : base(test){ }", false, "test")]
        [TestCase(@"public void TestMethod(){ }", false, "test")]
        public void ArgumentCheck_Should_Return_CorrectResponse(string code, bool shouldBeVaild, string args) {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var method = root.Members[0] as MethodDeclarationSyntax;

            if (method == null) {
                Assert.Fail();
            }

            Assert.AreEqual(shouldBeVaild, new Method(method, null).CheckIfArgumentsExists(args));
        }
    }
}
