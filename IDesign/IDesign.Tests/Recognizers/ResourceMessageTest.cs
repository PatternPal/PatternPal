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
        public void ResourceMessage_Should_Be(string key, string parameter, string resourceMessage)
        {
          var rs =   new ResourceMessage(key, new[] { parameter });
            var checkr = new CheckResult(rs, FeedbackType.Correct, null, 1f);
            Assert.AreEqual(resourceMessage, ResourceUtils.ResultToString(checkr));
        }
    }
}
