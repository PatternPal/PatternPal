using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.Decorator.DecoratorTestCase1
{
    class ConcreteDecorator2 : Decorator
    {
        public ConcreteDecorator2(IComponent component) : base(component) { }

        public override int Operation()
        {
            return base.Operation() + 1;
        }
    }
}
