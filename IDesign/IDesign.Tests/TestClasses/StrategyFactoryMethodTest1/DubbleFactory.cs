using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StrategyFactoryMethodTest1
{
    //Context class
     class DubbleFactory : BeerFactory
    {
        public override IBeer BrewBier()
        {
            return new Dubbel();
        }

        private IStrategy _strategy;

        public DubbleFactory()
        { }

        public DubbleFactory(IStrategy strategy)
        {
            this._strategy = strategy;
        }

        public void SetStrategy(IStrategy strategy)
        {
            this._strategy = strategy;
        }


        public void DoSomeBusinessLogic()
        {
            Console.WriteLine("Context: Sorting data using the strategy (not sure how it'll do it)");
            var result = this._strategy.DoAlgorithm(new List<string> { "a", "b", "c", "d", "e" });

            string resultStr = string.Empty;
            foreach (var element in result as List<string>)
            {
                resultStr += element + ",";
            }

            Console.WriteLine(resultStr);
        }
    }
}
