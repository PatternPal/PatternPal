namespace PatternPal.Tests.TestClasses.FactoryMethodTest
{
    //Context class
    internal class DubbleFactory : BeerFactory
    {
        public override IBeer BrewBier()
        {
            return new Dubbel();
        }
    }
}
