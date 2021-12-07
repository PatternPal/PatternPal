using System;

namespace IDesign.Tests.TestClasses.ObserverTest4
{
    internal class ObserverA : IObserver
    {
        public void Update()
        {
            Console.WriteLine("very nice");
        }
    }
}
