using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.Decorator.DecoratorTestCase1
{
    class ConcreteDecorator : Decorator
    {
        public ConcreteDecorator(IComponent component) : base(component) { }
    }
}
