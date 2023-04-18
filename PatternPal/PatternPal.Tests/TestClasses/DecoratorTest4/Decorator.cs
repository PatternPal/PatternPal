namespace PatternPal.Tests.TestClasses.DecoratorTest4
{
    internal abstract class Decorator : IComponent
    {
        private IComponent component;

        public Decorator(IComponent component)
        {
            this.component = component;
        }

        public void Operation()
        {
            throw new NotImplementedException();
        }
    }
}
