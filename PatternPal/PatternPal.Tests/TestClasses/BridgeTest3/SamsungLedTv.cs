using System;

namespace PatternPal.Tests.TestClasses.BridgeTest3
{
    public class SamsungLedTv : LEDTV
    {
        public void SwitchOn()
        {
            Console.WriteLine("Turning ON : Samsung TV");
        }

        public void SwitchOff()
        {
            Console.WriteLine("Turning OFF : Samsung TV");
        }

        public void SetChannel(int channelNumber)
        {
            Console.WriteLine("Setting channel Number " + channelNumber + " on Samsung TV");
        }
    }
}
