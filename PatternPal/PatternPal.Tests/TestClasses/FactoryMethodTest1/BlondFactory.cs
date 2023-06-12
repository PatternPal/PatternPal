namespace PatternPal.Tests.TestClasses.FactoryMethodTest
{
    internal class BlondFactory : BeerFactory
    {
        public override IBeer BrewBier()
        {
            return new Blond();
        }
    }
}
