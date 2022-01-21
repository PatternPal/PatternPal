namespace PatternPal.Tests.Core.TestClasses
{
    internal class TestClass1 : ITest
    {
        public TestClass1(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public int Sum()
        {
            return X + Y;
        }
    }
}
