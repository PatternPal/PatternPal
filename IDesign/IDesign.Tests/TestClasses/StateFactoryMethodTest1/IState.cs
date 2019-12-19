using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StateFactoryMethodTest1
{
    interface IState
    {
        void Handle(DubbleFactory context);
    }
}
