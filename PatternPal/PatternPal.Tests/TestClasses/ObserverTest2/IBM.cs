namespace PatternPal.Tests.TestClasses.ObserverTest2
{
    //This code is from https://www.dofactory.com/net/observer-design-pattern

    /// <summary>
    ///     The 'ConcreteSubject' class
    /// </summary>
    internal class IBM : Stock
    {
        // Constructor
        public IBM(string symbol, double price)
            : base(symbol, price)
        {
        }
    }
}
