using System;

namespace IDesign.Tests.TestClasses.ObserverTest3
{
    class ObserverA : IObserver
    {
        public void Update()
        {
            Console.WriteLine("very nice");
        }
    }
}
