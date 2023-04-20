using System;
using System.Collections.Generic;
using System.Text;

namespace PatternPal.Tests.TestClasses.Strategy
{
    /* Pattern:              Strategy
     * Original code source: https://codewithshadman.com/strategy-pattern-csharp/
     * 
     * 
     * Requirements to fullfill the pattern:
     *         Strategy interface
     *            ✓  a) is an interface	/ abstract class
     *            ✓  b) has declared a method
     *            ✓        1) if the class is an abstract instead of an interface the method has to be an abstract method
     *            ✓  c) is used by another class
     *            ✓  d) is implemented / inherited by at least one other class
     *            ✓  e) is implemented / inherited by at least two other classes
     *         Concrete strategy
     *            ✓  a) is an implementation of the Strategy interface
     *            ✓  b) if the class is used, it must be used via the context class
     *            ✓  c) if the class is not used it should be used via the context class
     *               d) is stored in the context class
     *         Context
     *            ✓  a) has a private field or property that has a Strategy class as type 
     *               b) has a function setStrategy() to set the non-public field / property with parameter of type Strategy
     *            ✓  c) has a function useStrategy() to execute the strategy. 
     *         Client
     *            ✓  a) has created an object of the type ConcreteStrategy
     *               b) has used the setStrategy() in the Context class to store the ConcreteStrategy object
     *               c) has executed the ConcreteStrategy via the Context class
     */

    //Strategy interface
    interface IOfferStrategy
    {
        string Name { get; }
        double GetDiscountPercentage();
    }

    //Concrete strategy
    class NoDiscountStrategy : IOfferStrategy
    {
        public string Name => nameof(NoDiscountStrategy);

        public double GetDiscountPercentage()
        {
            return 0;
        }
    }

    //Concrete strategy
    class QuarterDiscountStrategy : IOfferStrategy
    {
        public string Name => nameof(QuarterDiscountStrategy);

        public double GetDiscountPercentage()
        {
            return 0.25;
        }
    }

    //Context
    class StrategyContext
    {
        private IOfferStrategy _strategyUnused;

        double price; // price for some item or air ticket etc.
        Dictionary<string, IOfferStrategy> strategyContext
            = new Dictionary<string, IOfferStrategy>();
        public StrategyContext(double price)
        {
            this.price = price;
            strategyContext.Add(nameof(NoDiscountStrategy),
                new NoDiscountStrategy());
            strategyContext.Add(nameof(QuarterDiscountStrategy),
                new QuarterDiscountStrategy());
        }

        public void ApplyStrategy(IOfferStrategy strategy)
        {
            /*
            Currently applyStrategy has simple implementation. 
            You can Context for populating some more information,
            which is required to call a particular operation
            */
            Console.WriteLine("Price before offer :" + price);
            double finalPrice
                = price - (price * strategy.GetDiscountPercentage());
            Console.WriteLine("Price after offer:" + finalPrice);
        }

        public IOfferStrategy GetStrategy(int monthNo)
        {
            /*
            In absence of this Context method, client has to import 
            relevant concrete Strategies everywhere.
            Context acts as single point of contact for the Client 
            to get relevant Strategy
            */
            if (monthNo < 6)
            {
                return strategyContext[nameof(NoDiscountStrategy)];
            }
            else
            {
                return strategyContext[nameof(QuarterDiscountStrategy)];
            }
        }
    }

    //Client
    file class Program
    {
        static void Main13(string[] args)
        {
            StrategyContext context = new StrategyContext(100);
            Console.WriteLine("Enter month number between 1 and 12");
            var input = Console.ReadLine();
            int month = Convert.ToInt32(input);
            Console.WriteLine("Month =" + month);
            IOfferStrategy strategy = context.GetStrategy(month);

            Console.ReadLine();
        }
    }
}
