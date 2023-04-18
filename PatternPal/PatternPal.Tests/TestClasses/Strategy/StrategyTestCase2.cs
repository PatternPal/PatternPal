using System.Collections.Generic;
using System.Text;

namespace PatternPal.Tests.TestClasses.Strategy
{
    //this code is from https://refactoring.guru/design-patterns/strategy/csharp/example

    /*This test is a possible "perfect" Strategy implementation, which is not used by a client.
     * 1- It includes an interface
     * 2- With a method
     * 3- And is inherited by at least two classes
     */
    public interface IStrategy
    {
        object DoAlgorithm(object data);
    }

    /*
     * 4- It includes a class which inherits from the interface
     * 5- And implements its method
     */
    internal class ConcreteStrategyA : IStrategy
    {
        public object DoAlgorithm(object data)
        {
            var list = data as List<string>;
            list.Sort();

            return list;
        }
    }

    /*
    * 6- It includes a second class which inherits from the interface
    * 7- And implements its method
    */
    internal class ConcreteStrategyB : IStrategy
    {
        public object DoAlgorithm(object data)
        {
            var list = data as List<string>;
            list.Sort();
            list.Reverse();

            return list;
        }
    }

    /*
    * 10- It includes a class which has a private field with as type the interface
    * 11- And has a function to set that field to a concrete strategy
    * 12- And has a function in which the field is used.
    * 13- It has no direct calls to a concrete strategy.
    */
    internal class Context
    {
        private IStrategy _strategy;

        public Context()
        {
        }

        public Context(IStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IStrategy strategy)
        {
            _strategy = strategy;
        }

        public void DoSomeBusinessLogic()
        {
            Console.WriteLine("Context: Sorting data using the strategy (not sure how it'll do it)");
            var result = _strategy.DoAlgorithm(new List<string>
            {
                "a",
                "b",
                "c",
                "d",
                "e"
            });

            var resultStr = string.Empty;
            foreach (var element in result as List<string>)
            {
                resultStr += element + ",";
            }

            Console.WriteLine(resultStr);
        }
    }

    /*
    * 14- It includes a class which has a private field with as type the context class
    * 15- And uses its method to set its strategy
    * 16- And uses its method to use its strategy
    * MAYBE IT WOULD BE SMART TO MAKE THIS IMPLEMENTATION MORE LOGICAL; AKA ADD A WAY TO "CHOOSE" THE DESIRED STRATEGY
    */
    /*internal class Client
    {
        private Context _context;
        public Client()
        {
            _context = new Context();
            _context.SetStrategy(new ConcreteStrategyA());
            _context.DoSomeBusinessLogic();
        }
    }*/
}
