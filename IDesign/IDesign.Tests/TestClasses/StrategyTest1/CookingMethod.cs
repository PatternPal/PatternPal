using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StrategyTest1
{
    //this code is from https://github.com/exceptionnotfound/DesignPatterns/blob/master/Strategy/CookMethod.cs

    /// <summary>
    ///     The Context class, which maintains a reference to the chosen Strategy.
    /// </summary>
    class CookingMethod
    {
        private string Food;
        private CookStrategy _cookStrategy;

        public void SetCookStrategy(CookStrategy cookStrategy)
        {
            this._cookStrategy = cookStrategy;
        }

        public void SetFood(string name)
        {
            Food = name;
        }

        public void Cook()
        {
            _cookStrategy.Cook(Food);
            Console.WriteLine();
        }
    }
}
