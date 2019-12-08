using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using IDesign.Recognizers;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models;

namespace IDesign.Tests.Checks
{
    public class PropertyTest
    {
        [Test]
        [TestCase("public", @"public string TestProperty{get; set;}", true)]
        [TestCase("private", @"private int TestProperty{get; set;}", true)]
        [TestCase("public", @"public static Class TestProperty{get; set;}", true)]
        [TestCase("static", @"private static Class<T> TestProperty{get; set;}", true)]
        [TestCase("private", @"static Class[] TestProperty{get; set;}", false)]
        [TestCase("public", @"private var TestProperty{get; set;}", false)]
        [TestCase("static", @"public bool TestProperty{get; set;}", false)]
        public void ModifierCheck_Should_Return_CorrectResponse(string modifier, string code, bool shouldBeValid)
        {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var property = root.Members[0] as PropertyDeclarationSyntax;

            if (property == null)
                Assert.Fail();

            Assert.AreEqual(shouldBeValid, new PropertyField(property).CheckMemberModifier(modifier));
        }

        [Test]
        [TestCase("string", @"public string TestProperty{get; set;}", true)]
        [TestCase("int", @"private int TestProperty{get; set;}", true)]
        [TestCase("Class", @"public static Class TestProperty{get; set;}", true)]
        [TestCase("Class<T>", @"private static Class<T> TestProperty{get; set;}", true)]
        [TestCase("var", @"static var TestProperty{get; set;}", true)]
        [TestCase("T", @"private var TestProperty{get; set;}", false)]
        [TestCase("int", @"public bool TestProperty{get; set;}", false)]
        [TestCase("int",
            @"public static Singleton Instance { get { if (instance==null) { instance = new Singleton(); } return instance; } } }",
            false)]
        public void TypeCheck_Should_Return_CorrectResponse(string type, string code, bool shouldBeValid)
        {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var property = root.Members[0] as PropertyDeclarationSyntax;

            if (property == null)
                Assert.Fail();

            Assert.AreEqual(shouldBeValid, new PropertyField(property).CheckFieldType(type));
        }
    }
}