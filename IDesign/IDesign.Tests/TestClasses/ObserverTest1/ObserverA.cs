using System;

namespace IDesign.Tests.TestClasses.ObserverTest1
{
    internal class ObserverA : IObserver
    {
        public void Update()
        {
            Console.WriteLine("very nice");
        }
    }
}
