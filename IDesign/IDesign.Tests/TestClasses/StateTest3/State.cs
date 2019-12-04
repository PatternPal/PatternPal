using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StateTest3
{
    /// <summary>
    /// The 'State' abstract class
    /// </summary>
    abstract class State
    {
        public abstract void Handle(Context context);
    }
}
