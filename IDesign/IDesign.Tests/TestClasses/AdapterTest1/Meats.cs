using System;

namespace IDesign.Tests.TestClasses.AdapterTest1
{
    /// <summary>

    /// The 'Target' class

    /// </summary>

    class Target

    {
        public virtual void Request()
        {
            Console.WriteLine("Called Target Request()");
        }
    }

    /// <summary>

    /// The 'Adapter' class

    /// </summary>

    class Adapter : Target

    {
        private Adaptee _adaptee = new Adaptee();

        public override void Request()
        {
            // Possibly do some other work

            //  and then call SpecificRequest

            _adaptee.SpecificRequest();
        }
    }

    /// <summary>

    /// The 'Adaptee' class

    /// </summary>

    class Adaptee

    {
        public void SpecificRequest()
        {
            Console.WriteLine("Called SpecificRequest()");
        }
    }
}
