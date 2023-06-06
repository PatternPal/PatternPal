using System;
using System.Collections.Generic;
using System.Text;

namespace PatternPal.Tests.TestClasses.Strategy
{
    /* Pattern:              Strategy
     * Original code source: https://www.dofactory.com/net/strategy-design-pattern
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
    file abstract class Strategy
    {
        public abstract void AlgorithmInterface();
    }

    //Concrete strategy
    file class ConcreteA : Strategy
    {
        public override void AlgorithmInterface()
        {
            Console.WriteLine("Called ConcreteA.AlgorithmInterface()");
        }
    }

    //Concrete strategy
    file class ConcreteB : Strategy
    {
        public override void AlgorithmInterface()
        {
            Console.WriteLine("Called ConcreteB.AlgorithmInterface()");
        }
    }

    //Concrete strategy
    file class ConcreteC : Strategy
    {
        public override void AlgorithmInterface()
        {
            Console.WriteLine("Called ConcreteC.AlgorithmInterface()");
        }
    }

    //Context
    file class TheContext
    {
        private Strategy _strategy;

        public TheContext(Strategy strategy)
        {
            _strategy = strategy;
        }

        public TheContext()
        {

        }

        public void SetStrategy(Strategy strategy)
        {
            _strategy = strategy;
        }

        public void ContextInterface()
        {
            _strategy.AlgorithmInterface();
        }
    }

    //Client
    file class TheClient
    {
        private string input = "A";
        private TheContext _theContext;
        public TheClient()
        {
            _theContext = new TheContext();
            _theContext.SetStrategy(input switch
            {
                "A" => new ConcreteA(),
                "B" => new ConcreteB(),
                _ => new ConcreteC()
            });

            _theContext.ContextInterface();
        }
    }
}
