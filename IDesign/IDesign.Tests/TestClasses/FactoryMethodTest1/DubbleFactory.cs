namespace IDesign.Tests.TestClasses.FactoryMethodTest1
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
