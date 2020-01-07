using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.ObserverTest1
{
    class ObserverA : IObserver
    {
        public void Update()
        {
            Console.WriteLine("very nice");
        }
    }
}
