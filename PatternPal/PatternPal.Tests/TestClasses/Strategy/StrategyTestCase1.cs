using System;
using System.Collections.Generic;
using System.Text;

namespace PatternPal.Tests.TestClasses.Strategy
{
    //this code is from https://github.com/exceptionnotfound/DesignPatterns/blob/master/Strategy/CookMethod.cs

    /*This test is a possible "perfect" Strategy implementation.
     * 1- It includes an abstract class
     * 2- With an abstract method
     * 3- And is inherited by at least two classes
     */
    internal abstract class CookStrategy
    {
        public abstract void Cook(string food);
    }

   /*
    * 4- It includes a class which inherits from the abstract class
    * 5- And implements its method
    */
    internal class OvenBaking : CookStrategy
    {
        public override void Cook(string food)
        {
            Console.WriteLine("\nCooking " + food + " by oven baking it.");
        }
    }

    /*
    * 6- It includes a second class which inherits from the abstract class
    * 7- And implements its method
    */
    internal class Grilling : CookStrategy
    {
        public override void Cook(string food)
        {
            Console.WriteLine("\nCooking " + food + " by grilling it.");
        }
    }

    /*
     * 8- It includes a third class which inherits from the abstract class
     * 9- And implements its method
     */
    internal class DeepFrying : CookStrategy
    {
        public override void Cook(string food)
        {
            Console.WriteLine("\nCooking " + food + " by deep frying it");
        }
    }

    /*
    * 10- It includes a class which has a private field with as type the abstract class
    * 11- And has a function to set that field to a concrete strategy
    * 12- And has a function in which the field is used.
    * 13- It has no direct calls to a concrete strategy.
    */
    internal class CookingMethod
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
            _cookStrategy.Cook(_food);
            Console.WriteLine();
        }
    }
    /*
    * 14- It includes a class which has a private field with as type the context class
    * 15- And uses its method to set its strategy
    * 16- And uses its method to use its strategy
    * MAYBE IT WOULD BE SMART TO MAKE THIS IMPLEMENTATION MORE LOGICAL; AKA ADD A WAY TO "CHOOSE" THE DESIRED STRATEGY
    */
    internal class Kitchen
    {
        private CookingMethod cookingMethod;
        public Kitchen()
        {
            cookingMethod = new CookingMethod();
            cookingMethod.SetCookStrategy(new DeepFrying());
            cookingMethod.SetFood("pizza");
            cookingMethod.Cook();
        }
    }
}
