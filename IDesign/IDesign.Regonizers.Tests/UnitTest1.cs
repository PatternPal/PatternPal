using NUnit.Framework;

namespace IDesign.Regonizers.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var s = new SingletonRegonizer();
            
            Assert.IsNull(s.Regonize());
        }
    }
}