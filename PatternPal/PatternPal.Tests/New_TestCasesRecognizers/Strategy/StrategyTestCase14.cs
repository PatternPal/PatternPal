using System;
using System.Collections.Generic;
using System.Text;

namespace PatternPal.Tests.TestClasses.Strategy
{
    /* Pattern:              Strategy
     * Original code source: https://methodpoet.com/strategy-pattern/
     * 
     * 
     * Requirements to fullfill the pattern:
     *         Strategy interface
     *            ✓  a) is an interface	/ abstract class
     *               b) has declared a method
     *                     1) if the class is an abstract instead of an interface the method has to be an abstract method
     *            ✓  c) is used by another class
     *            ✓  d) is implemented / inherited by at least one other class
     *            ✓  e) is implemented / inherited by at least two other classes
     *         Concrete strategy
     *            ✓  a) is an implementation of the Strategy interface
     *               b) if the class is used, it must be used via the context class
     *            ✓  c) if the class is not used it should be used via the context class
     *            ✓  d) is stored in the context class
     *         Context
     *            ✓  a) has a private field or property that has a Strategy class as type 
     *            ✓  b) has a function setStrategy() to set the non-public field / property with parameter of type Strategy
     *               c) has a function useStrategy() to execute the strategy. 
     *         Client
     *            ✓  a) has created an object of the type ConcreteStrategy
     *            ✓  b) has used the setStrategy() in the Context class to store the ConcreteStrategy object
     *               c) has executed the ConcreteStrategy via the Context class
     */

    //Strategy interface
    public interface InterfaceStrategy
    {

    }

    //Concrete strategy
    class DefaultConcreteStrategy : InterfaceStrategy
    {
        public IEnumerable<string> PerformAlgorithm(List<string> list)
        {
            list.Sort();
            return list;
        }
    }

    //Concrete strategy
    class AlternativeConcreteStrategy : InterfaceStrategy
    {
        public IEnumerable<string> PerformAlgorithm(List<string> list)
        {
            list.Sort();
            list.Reverse();
            return list;
        }
    }

    //Context
    class Context
    {
        private InterfaceStrategy _strategy;

        public Context()
        {
        }

        public Context(InterfaceStrategy strategy)
        {
            _strategy = strategy;
        }

        // here we can replace the current or default strategy if we choose
        public void SetStrategy(InterfaceStrategy strategy)
        {
            _strategy = strategy;
        }
        public void CarryOutWork()
        {
            Console.WriteLine("Context: Carrying out Sorting Work");
        }
    }

    //Client
    class Program
    {
        static void Main(string[] args)
        {
            // client picks the default concrete strategy:
            Console.WriteLine("Sorting strategy has been set to alphabetical:");
            var context = new Context();

            DefaultConcreteStrategy strategy = new DefaultConcreteStrategy();
            context.SetStrategy(strategy);
            strategy.PerformAlgorithm(new List<string>{"A", "B", "C"});
            Console.WriteLine();

            // client picks the alternative concrete strategy
            Console.WriteLine("Sorting strategy has been set to reverse:");
            context.SetStrategy(new AlternativeConcreteStrategy());
            context.CarryOutWork();
        }
    }
}
