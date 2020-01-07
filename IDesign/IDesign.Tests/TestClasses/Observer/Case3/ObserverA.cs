using System;

namespace IDesign.Tests.TestClasses.Observer.Case3
{
    class ObserverA : IObserver
    {
        public void Update()
        {
            Console.WriteLine("very nice");
        }
    }
}
