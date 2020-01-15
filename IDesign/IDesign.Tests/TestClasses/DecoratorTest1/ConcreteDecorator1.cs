using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.DecoratorTest1
{
    class ConcreteDecorator1 : Decorator
    {
        public ConcreteDecorator1(IComponent component) : base(component) { }

        public override int Operation()
        {
            return base.Operation();
        }
    }
}
