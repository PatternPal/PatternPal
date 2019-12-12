using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.Decorator.DecoratorTestCase5
{
    class ConcreteDecorator1 : Decorator
    {
        public ConcreteDecorator1(IComponent component) : base(component) { }
    }
}
