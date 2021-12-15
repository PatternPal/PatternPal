using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.BridgeTest3
{
    public class SamsungRemoteControl : AbstractRemoteControl
    {
        public SamsungRemoteControl(LEDTV ledTv) : base(ledTv)
        {
        }

        public override void SwitchOn()
        {
            ledTv.SwitchOn();
        }

        public override void SwitchOff()
        {
            ledTv.SwitchOff();
        }

        public override void SetChannel(int channelNumber)
        {
            ledTv.SetChannel(channelNumber);
        }
    }
}
