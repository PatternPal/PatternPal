using System.Collections.Generic;
using IDesign.Recognizers.Checks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using SyntaxTree.Models.Members.Field;

namespace IDesign.Tests.Checks {
    public class FieldChecksTest {
        [Test]
        [TestCase("public", @"public string TestProperty", true)]
        [TestCase("private", @"private int TestProperty", true)]
        [TestCase("public", @"public static Class TestProperty", true)]
        [TestCase("static", @"private static Class<T> TestProperty", true)]
        [TestCase("private", @"static Class[] TestProperty", false)]
        [TestCase("public", @"private var TestProperty", false)]
        [TestCase("static", @"public bool TestProperty", false)]
        public void CheckModifier_Should_Return_CorrectResponse(string modifier, string code, bool shouldBeValid) {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var field = root.Members[0] as FieldDeclarationSyntax;

            if (field == null) {
                Assert.Fail();
            }

            Assert.AreEqual(
                shouldBeValid,
                new Field(field, null).CheckMemberModifier(modifier)
            );
        }

        [Test]
        [TestCase("string", @"public string TestProperty", true)]
        [TestCase("int", @"private int TestProperty", true)]
        [TestCase("Class", @"public static Class TestProperty", true)]
        [TestCase("Class<T>", @"private static Class<T> TestProperty", true)]
        [TestCase("var", @"static var TestProperty", true)]
        [TestCase("T", @"private var TestProperty", false)]
        [TestCase("int", @"public bool TestProperty", false)]
        public void TypeCheck_Should_Return_CorrectResponse(string type, string code, bool shouldBeValid) {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var field = root.Members[0] as FieldDeclarationSyntax;

            if (field == null) {
                Assert.Fail();
            }

            Assert.AreEqual(
                shouldBeValid,
                new Field(field, null).CheckFieldType(new List<string>() { type })
            );
        }
    }
}
