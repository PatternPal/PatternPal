namespace IDesign.Tests.TestClasses.DecoratorTest1
{
    internal class ConcreteDecorator1 : Decorator
    {
        public ConcreteDecorator1(IComponent component) : base(component) { }

        public override int Operation()
        {
            return base.Operation();
        }
    }
}
