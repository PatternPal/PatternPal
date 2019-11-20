using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDesign.Regonizers.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var s = new SingletonRegonizer();
            Assert.IsNull(s.Regonize());
        }
    }
}
