using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using IDesign.Checks;

namespace IDesign.Regonizers.Tests
{
    public class PropertyTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("public", @"public void TestProperty{get; set;}", true)]
        [TestCase("private", @"private int TestProperty{get; set;}", true)]
        [TestCase("public", @"public static Class TestProperty{get; set;}", true)]
        [TestCase("private", @"private static Class<T> TestProperty{get; set;}", true)]
        [TestCase("private", @"static Class[] TestProperty{get; set;}", false)]
        [TestCase("public", @"private void TestProperty{get; set;}", false)]
        [TestCase("static", @"public bool TestProperty{get; set;}", false)]
        public void ModifierCheck_Returns_CorrectResponse(string modifier, string code, bool shouldBeValid)
        {
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var method = root.Members[0] as MemberDeclarationSyntax;

            if (method == null)
                Assert.Fail();

            Assert.AreEqual(method.CheckMemberModifier(modifier), shouldBeValid);
        }
    }
}