using System;
using System.Collections.Generic;
using System.Text;

namespace PatternPal.Tests.TestClasses.Strategy
{
    //this code is from https://www.dofactory.com/net/strategy-design-pattern

    /*This test is a possible "perfect" Strategy implementation.
     * 1- It includes an abstract class
     * 2- With an abstract method
     * 3- And is inherited by at least two classes
     */
    internal abstract class Strategy
    {
        public abstract void AlgorithmInterface();
    }

    /*
     * 4- It includes a class which inherits from the abstract class
     * 5- And implements its method
     */
    internal class ConcreteA : Strategy
    {
        public override void AlgorithmInterface()
        {
            Console.WriteLine("Called ConcreteA.AlgorithmInterface()");
        }
    }

    /*
    * 6- It includes a second class which inherits from the abstract class
    * 7- And implements its method
    */
    internal class ConcreteB : Strategy
    {
        public override void AlgorithmInterface()
        {
            Console.WriteLine("Called ConcreteB.AlgorithmInterface()");
        }
    }

    /*
     * 8- It includes a third class which inherits from the abstract class
     * 9- And implements its method
     */
    internal class ConcreteC : Strategy
    {
        public override void AlgorithmInterface()
        {
            Console.WriteLine("Called ConcreteC.AlgorithmInterface()");
        }
    }

    /*
    * 10- It includes a class which has a private field with as type the abstract class
    * 11- The strategy must be set in the constructor IS THIS CORRECT?????????????????????
    * 12- And has a function in which the field is used.
    * 13- It has no direct calls to a concrete strategy.
    */
    internal class TheContext
    {
        private readonly Strategy _strategy;

        public TheContext(Strategy strategy)
        {
            _strategy = strategy;
        }

        public void ContextInterface()
        {
            _strategy.AlgorithmInterface();
        }
    }

    /*
    * 14- It includes a class which has a private field with as type the context class
    * 15- and sets the strategy using the constructor
    * 16- And uses its method to use its strategy
    * MAYBE IT WOULD BE SMART TO MAKE THIS IMPLEMENTATION MORE LOGICAL; AKA ADD A WAY TO "CHOOSE" THE DESIRED STRATEGY
    */
    internal class TheClient
    {
        private TheContext _theContext;
        public TheClient()
        {
            _theContext = new TheContext(new ConcreteA());
            _theContext.ContextInterface();
        }
    }
}
