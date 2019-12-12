namespace IDesign.Tests.TestClasses.Factory
{
    public class FactoryTestCase4
    {
        public ProductA Create()
        {
            return new ProductB();
        }
    }
}