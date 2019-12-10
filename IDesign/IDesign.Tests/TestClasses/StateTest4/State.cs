using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StateTest4
{
    //this code is from http://gyanendushekhar.com/2016/11/05/state-design-pattern-c/

    // 'State' interface
    public interface State
    {
        void ExecuteCommand(Player player);
    }
}
