using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.DecoratorTest5
{
    public class ConcreteComponent : IComponent
    {
        public ConcreteComponent() { }

        public int Operation()
        {
            return 1;
        }
    }
}
