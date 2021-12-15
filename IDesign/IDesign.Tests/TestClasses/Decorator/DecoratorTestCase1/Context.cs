namespace IDesign.Tests.TestClasses.Decorator.DecoratorTestCase1
{
    internal class Context
    {
        private void Main()
        {
            IComponent component = new ConcreteComponent();
            var decorator = new ConcreteDecorator1(component);
            Decorator d = new ConcreteDecorator1(decorator);
        }
    }
}
