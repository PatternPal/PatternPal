using PatternPal.CommonResources;
using PatternPal.Recognizers.Abstractions;
using PatternPal.Recognizers.Models.Output;
using NUnit.Framework;

namespace PatternPal.Tests.Recognizers
{
    [TestFixture]
    public class ResourceMessageTest
    {
        [Ignore("Old Tests, ignored for CI")]
        [Test]
        [TestCase("MethodReturnType", "BeerFactory", "Method should return BeerFactory.")]
        [TestCase("MethodReturnType", "Hello", "Method should return Hello.")]
        [TestCase("NodeAbstractOrInterface", "TestClass", "TestClass should be an abstract class or an interface.")]
        [TestCase("NodeAbstractOrInterface", "BeerFactory", "BeerFactory should be an abstract class or an interface.")]
        public void ResourceMessage_Should_Be_Correct_String(string key, string parameter, string resourceMessage)
        {
            var rs = new ResourceMessage(key, new[] {parameter});
            var checkr = new CheckResult(rs, FeedbackType.Correct, null, 1f);
            Assert.AreEqual(resourceMessage, ResourceUtils.ResultToString(checkr));
        }
    }
}
