using System;

namespace PatternPal.Tests.TestClasses.DecoratorTest5
{
    internal abstract class Decorator : IComponent
    {
        private IComponent component;

        public Decorator(IComponent component)
        {
            this.component = component;
        }

        public int Operation()
        {
            throw new NotImplementedException();
        }
    }
}
