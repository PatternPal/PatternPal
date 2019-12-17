using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.Decorator.DecoratorTestCase1
{
    class ConcreteDecorator1 : Decorator
    {
        public ConcreteDecorator1(IComponent component) : base(component) { }
    }
}
