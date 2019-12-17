using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.Decorator.DecoratorTestCase4
{
    abstract class Decorator : IComponent
    {
        IComponent component;

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
