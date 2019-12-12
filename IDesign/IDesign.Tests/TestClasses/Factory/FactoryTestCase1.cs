namespace IDesign.Tests.TestClasses.Factory
{
    public class FactoryTestCase1
    {
        public IProduct CreateProductA()
        {
            var product = new ProductA();
            return product;
        }
    }
}