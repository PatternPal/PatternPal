using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StateTest4
{
    // 'State' interface
    public interface State
    {
        void ExecuteCommand(Player player);
    }
}
