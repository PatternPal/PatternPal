namespace PatternPal.Tests.TestClasses.FactoryMethodTest3
{
    internal class ConcreteCreator1 : Creator
    {
        public override IProduct FactoryMethod()
        {
            return new ConcreteProduct1();
        }
    }
}
