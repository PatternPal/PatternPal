using System;
using System.Collections.Generic;
using System.Text;
using IDesign.CommonResources;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.Output;
using NUnit.Framework;

namespace IDesign.Tests.Recognizers
{
    public class ResourceMessageTest
    {
        [Test]
        [TestCase("MethodReturnType", "BeerFactory", "Method should return BeerFactory.")]
        [TestCase("MethodReturnType", "Hello", "Method should return Hello.")]
        [TestCase("NodeAbstractOrInterface", "", "Node should be an abstract class or an interface.")]
        [TestCase("NodeAbstractOrInterface", "BeerFactory", "Node should be an abstract class or an interface.")]
        public void ResourceMessage_Should_Be_Correct_String(string key, string parameter, string resourceMessage)
        {
          var rs =   new ResourceMessage(key, new[] { parameter });
            var checkr = new CheckResult(rs, FeedbackType.Correct, null, 1f);
            Assert.AreEqual(resourceMessage, ResourceUtils.ResultToString(checkr));
        }
    }
}
