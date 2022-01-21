using System.Collections.Generic;

namespace PatternPal.Tests.TestClasses.StrategyTest2
{
    //this code is from https://refactoring.guru/design-patterns/strategy/csharp/example

    internal class ConcreteStrategyB : IStrategy
    {
        public object DoAlgorithm(object data)
        {
            var list = data as List<string>;
            list.Sort();
            list.Reverse();

            return list;
        }
    }
}
