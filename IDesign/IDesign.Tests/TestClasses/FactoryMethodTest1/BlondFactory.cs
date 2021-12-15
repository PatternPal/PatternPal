namespace IDesign.Tests.TestClasses.FactoryMethodTest1
{
    internal class BlondFactory : BeerFactory
    {
        public override IBeer BrewBier()
        {
            return new Blond();
        }
    }
}
