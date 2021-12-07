namespace IDesign.Tests.TestClasses.StrategyFactoryMethodTest1
{
    internal class BlondFactory : BeerFactory
    {
        public override IBeer BrewBier()
        {
            return new Blond();
        }
    }
}
