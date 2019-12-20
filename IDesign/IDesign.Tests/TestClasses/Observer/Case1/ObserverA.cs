using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.Observer.Case1
{
    class ObserverA : IObserver
    {
        public void Update()
        {
            Console.WriteLine("very nice");
        }
    }
}
