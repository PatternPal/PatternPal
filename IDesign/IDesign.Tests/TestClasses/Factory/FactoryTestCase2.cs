namespace IDesign.Tests.TestClasses.Factory
{
    public class FactoryTestCase2
    {
        public ProductA CreateProductA()
        {
            var product = new ProductA();
            return product;
        }
    }
}