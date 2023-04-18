namespace PatternPal.Tests.TestClasses.AdapterTest1
{
    /// <summary>
    ///     The 'Target' class
    /// </summary>
    internal class Target
    {
        public virtual void Request()
        {
            Console.WriteLine("Called Target Request()");
        }
    }

    /// <summary>
    ///     The 'Adapter' class
    /// </summary>
    internal class Adapter : Target

    {
        private readonly Adaptee _adaptee = new Adaptee();

        public override void Request()
        {
            // Possibly do some other work
            //  and then call SpecificRequest
            _adaptee.SpecificRequest();
        }
    }

    /// <summary>
    ///     The 'Adaptee' class
    /// </summary>
    internal class Adaptee

    {
        public void SpecificRequest()
        {
            Console.WriteLine("Called SpecificRequest()");
        }
    }
}
