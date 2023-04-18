using System.Collections.Generic;

namespace PatternPal.Tests.TestClasses.StrategyFactoryMethodTest1
{
    //Context class
    internal class DubbleFactory : BeerFactory
    {
        private IStrategy _strategy;

        public DubbleFactory()
        {
        }

        public DubbleFactory(IStrategy strategy)
        {
            _strategy = strategy;
        }

        public override IBeer BrewBier()
        {
            return new Dubbel();
        }

        public void SetStrategy(IStrategy strategy)
        {
            _strategy = strategy;
        }


        public void DoSomeBusinessLogic()
        {
            Console.WriteLine("Context: Sorting data using the strategy (not sure how it'll do it)");
            var result = _strategy.DoAlgorithm(new List<string>
            {
                "a",
                "b",
                "c",
                "d",
                "e"
            });

            var resultStr = string.Empty;
            foreach (var element in result as List<string>)
            {
                resultStr += element + ",";
            }

            Console.WriteLine(resultStr);
        }
    }
}
