using System;

namespace IDesign.Tests.TestClasses.Decorator.DecoratorTestCase5
{
    abstract class Decorator : IComponent
    {
        IComponent component;

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
