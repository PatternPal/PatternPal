using System;

namespace IDesign.Tests.TestClasses.Observer.Case5
{
    class ObserverA : IObserver
    {
        public void Update()
        {
            Console.WriteLine("very nice");
        }
    }
}
