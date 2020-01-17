using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StateFactoryMethodTest1
{
    class ConcreteStateB : IState
    {
        public void Handle(DubbleFactory context)
        {
            context.State = new ConcreteStateA();
        }
    }
}
