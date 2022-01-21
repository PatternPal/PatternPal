using System;

namespace PatternPal.Tests.TestClasses.ObserverTest5
{
    internal class ObserverA : IObserver
    {
        public void Update()
        {
            Console.WriteLine("very nice");
        }
    }
}
