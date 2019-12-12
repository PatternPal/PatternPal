namespace IDesign.Tests.TestClasses.Factory
{
    public class FactoryTestCase3
    {
        private readonly ProductA product = new ProductA();

        public IProduct CreateProductA()
        {
            return product;
        }
    }
}