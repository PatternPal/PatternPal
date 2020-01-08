using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.DecoratorTest1
{
    class Context
    {
        void Main()
        {
            IComponent component = new ConcreteComponent();
            ConcreteDecorator1 decorator = new ConcreteDecorator1(component);
            Decorator d = new ConcreteDecorator1(decorator);
        }
    }
}
