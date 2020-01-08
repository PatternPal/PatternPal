namespace IDesign.Tests.Core.TestClasses
{
    internal class TestClass1 : ITest
    {
        public TestClass1(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public int Sum()
        {
            return X + Y;
        }
    }
}