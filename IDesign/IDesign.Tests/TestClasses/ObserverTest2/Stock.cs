using System;
using System.Collections.Generic;

namespace IDesign.Tests.TestClasses.ObserverTest2
{
    //This code is from https://www.dofactory.com/net/observer-design-pattern

    /// <summary>
    ///     The 'Subject' abstract class
    /// </summary>
    internal abstract class Stock
    {
        private readonly List<IInvestor> _investors = new List<IInvestor>();
        private double _price;

        // Constructor
        public Stock(string symbol, double price)
        {
            Symbol = symbol;
            _price = price;
        }

        // Gets or sets the price
        public double Price
        {
            get => _price;

            set
            {
                if (_price != value)
                {
                    _price = value;
                    Notify();
                }
            }
        }

        // Gets the symbol
        public string Symbol { get; }

        public void Attach(IInvestor investor)
        {
            _investors.Add(investor);
        }

        public void Detach(IInvestor investor)
        {
            _investors.Remove(investor);
        }

        public void Notify()
        {
            foreach (var investor in _investors)
            {
                investor.Update(this);
            }

            Console.WriteLine("");
        }
    }
}
