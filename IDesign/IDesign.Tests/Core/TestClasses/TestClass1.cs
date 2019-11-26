namespace IDesign.Tests.Core.TestClasses
{
    internal class TestClass1 : ITest
    {
        public TestClass1(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x { get; set; }
        public int y { get; set; }

        public int Sum()
        {
            return x + y;
        }
    }
}