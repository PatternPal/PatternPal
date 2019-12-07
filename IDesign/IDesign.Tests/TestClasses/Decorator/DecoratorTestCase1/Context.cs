using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.Decorator.DecoratorTestCase1
{
    class Context
    {
        void Main()
        {
            IComponent component = new ConcreteComponent();
            ConcreteDecorator1 decorator = new ConcreteDecorator1(component);
        }
    }
}
