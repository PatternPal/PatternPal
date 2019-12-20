using System;

namespace IDesign.Tests.TestClasses.Observer.Case4
{
    class ObserverA : IObserver
    {
        public void Update()
        {
            Console.WriteLine("very nice");
        }
    }
}
