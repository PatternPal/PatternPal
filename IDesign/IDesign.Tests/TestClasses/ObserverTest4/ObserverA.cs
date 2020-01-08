using System;

namespace IDesign.Tests.TestClasses.ObserverTest4
{
    class ObserverA : IObserver
    {
        public void Update()
        {
            Console.WriteLine("very nice");
        }
    }
}
