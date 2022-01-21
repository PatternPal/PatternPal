using System.Collections.Generic;
using PatternPal.Recognizers.Checks;
using PatternPal.Tests.Utils;
using NUnit.Framework;

namespace PatternPal.Tests.Checks
{
    public class FieldChecksTest
    {
        [Test]
        [TestCase("public", @"public string TestProperty", true)]
        [TestCase("private", @"private int TestProperty", true)]
        [TestCase("public", @"public static Class TestProperty", true)]
        [TestCase("static", @"private static Class<T> TestProperty", true)]
        [TestCase("private", @"static Class[] TestProperty", false)]
        [TestCase("public", @"private var TestProperty", false)]
        [TestCase("static", @"public bool TestProperty", false)]
        public void CheckModifier_Should_Return_CorrectResponse(string modifier, string code, bool shouldBeValid)
        {
            var field = EntityNodeUtils.CreateField(code);

            Assert.AreEqual(
                shouldBeValid,
                field.CheckMemberModifier(modifier)
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
        public void TypeCheck_Should_Return_CorrectResponse(string type, string code, bool shouldBeValid)
        {
            var field = EntityNodeUtils.CreateField(code);

            Assert.AreEqual(
                shouldBeValid,
                field.CheckFieldType(new List<string> {type})
            );
        }
    }
}
