using System;

namespace IDesign.Tests.TestClasses.StrategyTest1
{
    //this code is from https://github.com/exceptionnotfound/DesignPatterns/blob/master/Strategy/CookMethod.cs

    /// <summary>
    ///     The Context class, which maintains a reference to the chosen Strategy.
    /// </summary>
    internal class CookingMethod
    {
        private CookStrategy _cookStrategy;
        private string Food;

        public void SetCookStrategy(CookStrategy cookStrategy)
        {
            _cookStrategy = cookStrategy;
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
