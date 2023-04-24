using System;
using System.Collections.Generic;
using System.Text;

namespace PatternPal.Tests.TestClasses.Strategy
{
    /* Pattern:              Strategy
     * Original code source: https://github.com/exceptionnotfound/DesignPatterns/blob/master/Strategy/CookMethod.cs
     * 
     * 
     * Requirements to fullfill the pattern:
     *         Strategy interface
     *            ✓  a) is an interface	/ abstract class
     *            ✓  b) has declared a method
     *            ✓        1) if the class is an abstract instead of an interface the method has to be an abstract method
     *               c) is used by another class
     *            ✓  d) is implemented / inherited by at least one other class
     *            ✓  e) is implemented / inherited by at least two other classes
     *         Concrete strategy
     *            ✓  a) is an implementation of the Strategy interface
     *            ✓  b) if the class is used, it must be used via the context class
     *               c) if the class is not used it should be used via the context class
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
    file abstract class CookStrategy
    {
        public abstract void Cook(string food);
    }

    //Concrete strategy
    file class OvenBaking : CookStrategy
    {
        public override void Cook(string food)
        {
            Console.WriteLine("\nCooking " + food + " by oven baking it.");
        }
    }

    //Concrete strategy
    file class Grilling : CookStrategy
    {
        public override void Cook(string food)
        {
            Console.WriteLine("\nCooking " + food + " by grilling it.");
        }
    }

    //Context
    file class CookingMethod
    {
        private CookStrategy _cookStrategy;
        private string _food;

        public void SetCookStrategy(CookStrategy cookStrategy)
        {
            _cookStrategy = cookStrategy;
        }

        public void SetFood(string food)
        {
            _food = food;
        }

        public void Cook()
        {
            Console.WriteLine();
        }
    }

    //Client
    file class Kitchen
    {
        private CookingMethod cookingMethod;
        public Kitchen()
        {
            cookingMethod = new CookingMethod();
            cookingMethod.SetCookStrategy(new OvenBaking());
            cookingMethod.SetFood("pizza");
            cookingMethod.Cook();
            cookingMethod.SetCookStrategy(new Grilling());
            cookingMethod.SetFood("aubergine");
            cookingMethod.Cook();
        }
    }
}
