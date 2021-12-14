using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.BridgeTest3
{
    public interface LEDTV
    {
        void SwitchOn();
        void SwitchOff();
        void SetChannel(int channelNumber);
    }
}
