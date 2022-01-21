namespace PatternPal.Tests.TestClasses.Decorator.DecoratorTestCase1
{
    internal abstract class Decorator : IComponent
    {
        private readonly IComponent component;

        public Decorator(IComponent component)
        {
            this.component = component;
        }

        public virtual int Operation()
        {
            return component.Operation();
        }
    }
}
