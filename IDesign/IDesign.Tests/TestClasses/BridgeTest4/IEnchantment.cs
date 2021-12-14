using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.BridgeTest4
{
    public interface IEnchantment
    {
        void OnActivate();
        void Apply();
        void OnDeactivate();
    }
}
