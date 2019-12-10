namespace IDesign.Tests.TestClasses.Factory
{
    public class FactoryTestCase5
    {
        public IProduct Create()
        {
            return new ProductB();
        }
    }
}