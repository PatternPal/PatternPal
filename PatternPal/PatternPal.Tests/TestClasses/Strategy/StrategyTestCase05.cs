using System;
using System.Collections.Generic;
using System.Text;

namespace PatternPal.Tests.TestClasses.Strategy
{
    /* Pattern:              Strategy
         * Original code source: https://www.c-sharpcorner.com/UploadFile/shinuraj587/strategy-pattern-in-net/
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
         *               b) if the class is used, it must be used via the context class
         *            ✓  c) if the class is not used it should be used via the context class
         *            ✓  d) is stored in the context class
         *         Context
         *            ✓  a) has a private field or property that has a Strategy class as type 
         *            ✓  b) has a function setStrategy() to set the non-public field / property with parameter of type Strategy
         *            ✓  c) has a function useStrategy() to execute the strategy. 
         *         Client
         *            ✓  a) has created an object of the type ConcreteStrategy
         *            ✓  b) has used the setStrategy() in the Context class to store the ConcreteStrategy object
         *            ✓  c) has executed the ConcreteStrategy via the Context class
         */

    //Strategy interface
    public interface ICalculateInterface
    {
        //define method  
        int Calculate(int value1, int value2);
    }

    //Concrete strategy
    class Minus : ICalculateInterface
    {
        public int Calculate(int value1, int value2)
        {
            //define logic  
            return value1 - value2;
        }
    }

    //Concrete strategy
    class Plus : ICalculateInterface
    {
        public int Calculate(int value1, int value2)
        {
            //define logic  
            return value1 + value2;
        }
    }

    //Context
    class CalculateClient
    {
        private ICalculateInterface calculateInterface;

        //Constructor: assigns strategy to interface 
        public CalculateClient(ICalculateInterface strategy)
        {
            calculateInterface = strategy;
        }

        //assigns strategy to interface
        public void SetStrategy(ICalculateInterface strategy)
        {
            calculateInterface = strategy;
        }

        //Executes the strategy  
        public int Calculate(int value1, int value2)
        {
            return calculateInterface.Calculate(value1, value2);
        }

        public void UnusedMethod()
        {
            Minus minus = new Minus();
            minus.Calculate(0, 0);
        }
    }

    //Client
    class MainApp
    {
        static void EntryPoint()
        {
            CalculateClient client = new CalculateClient(new Minus());
            Console.Write("Minus: " + client.Calculate(7, 1).ToString());

            client.SetStrategy(new Plus());
            Console.Write("Plus: " + client.Calculate(7, 1).ToString());
            // Wait for user  
            Console.ReadKey();
        }
    }
}
