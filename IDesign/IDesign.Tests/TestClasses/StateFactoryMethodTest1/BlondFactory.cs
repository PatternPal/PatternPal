namespace IDesign.Tests.TestClasses.StateFactoryMethodTest1
{
    internal class BlondFactory : BeerFactory
    {
        public override IBeer BrewBier()
        {
            return new Blond();
        }
    }
}
