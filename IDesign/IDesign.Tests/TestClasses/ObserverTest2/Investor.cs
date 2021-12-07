using System;

namespace IDesign.Tests.TestClasses.ObserverTest2
{
    //This code is from https://www.dofactory.com/net/observer-design-pattern

    /// <summary>
    ///     The 'ConcreteObserver' class
    /// </summary>
    internal class Investor : IInvestor

    {
        private readonly string _name;

        // Constructor
        public Investor(string name)
        {
            _name = name;
        }

        // Gets or sets the stock
        public Stock Stock { get; set; }

        public void Update(Stock stock)
        {
            Console.WriteLine(
                "Notified {0} of {1}'s " +
                "change to {2:C}", _name, stock.Symbol, stock.Price
            );
        }
    }
}
