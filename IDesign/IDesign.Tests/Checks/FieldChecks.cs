using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using IDesign.Checks;

namespace IDesign.Tests.Checks
{
   public class FieldChecks
    { 
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("public", @"public string TestProperty", true)]
        [TestCase("private", @"private int TestProperty", true)]
        [TestCase("public", @"public static Class TestProperty", true)]
        [TestCase("static", @"private static Class<T> TestProperty", true)]
        [TestCase("private", @"static Class[] TestProperty", false)]
        [TestCase("public", @"private var TestProperty", false)]
        [TestCase("static", @"public bool TestProperty", false)]
        public void ModifierCheck_Should_Return_CorrectResponse(string modifier, string code, bool shouldBeValid)
        {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var property = root.Members[0] as MemberDeclarationSyntax;

            if (property == null)
                Assert.Fail();

            Assert.AreEqual(shouldBeValid, property.CheckMemberModifier(modifier));
        }

        [TestCase("string", @"public string TestProperty", true)]
        [TestCase("int", @"private int TestProperty", true)]
        [TestCase("Class", @"public static Class TestProperty", true)]
        [TestCase("Class<T>", @"private static Class<T> TestProperty", true)]
        [TestCase("var", @"static var TestProperty", true)]
        [TestCase("T", @"private var TestProperty", false)]
        [TestCase("int", @"public bool TestProperty", false)]
        public void TypeCheck_Should_Return_CorrectResponse(string type, string code, bool shouldBeValid)
        {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var property = root.Members[0] as FieldDeclarationSyntax;

            if (property == null)
                Assert.Fail();

            Assert.AreEqual(shouldBeValid, property.CheckPropertyType(type));
        }
    }
}
