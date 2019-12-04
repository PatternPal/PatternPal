using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StateTest3
{
    /// <summary>
    /// A 'ConcreteState' class
    /// </summary>
     class ConcreteStateB : State
    {
        public override void Handle(Context context)
        {
            context.State = new ConcreteStateA();
        }
    }
}
