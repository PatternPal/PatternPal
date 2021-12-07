namespace IDesign.Tests.TestClasses.FactoryMethodTest3
{
    internal class ConcreteCreator2 : Creator
    {
        public override IProduct FactoryMethod()
        {
            return new ConcreteProduct2();
        }
    }
}
