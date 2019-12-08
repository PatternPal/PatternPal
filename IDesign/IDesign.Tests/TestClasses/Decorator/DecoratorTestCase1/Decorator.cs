using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.Decorator.DecoratorTestCase1
{
    abstract class Decorator : IComponent
    {
        IComponent component;

        public Decorator(IComponent component)
        {
            this.component = component;
        }

        public virtual int Operation()
        {
            return component.Operation();
        }
    }
}
