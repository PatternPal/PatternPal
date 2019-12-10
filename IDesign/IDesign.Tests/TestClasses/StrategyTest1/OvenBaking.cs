using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StrategyTest1
{
    //this code is from https://github.com/exceptionnotfound/DesignPatterns/blob/master/Strategy/CookMethod.cs

    /// <summary>
    ///     A Concrete Strategy class
    /// </summary>
    class OvenBaking : CookStrategy
    {
        public override void Cook(string food)
        {
            Console.WriteLine("\nCooking " + food + " by oven baking it.");
        }
    }
}
