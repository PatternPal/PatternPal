using IDesign.Tests.TestClasses.StateTest3;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.Recognizers
{
 
    class StateRecognizerTest
    {
        [Test]
        public void Test()
        {
            // Setup context in a state
            Context c = new Context(new ConcreteStateA());

            // Issue requests, which toggles state
            c.Request();
            c.Request();
            c.Request();
            c.Request();

            // Wait for user
            Console.ReadKey();
        }
    }
}
