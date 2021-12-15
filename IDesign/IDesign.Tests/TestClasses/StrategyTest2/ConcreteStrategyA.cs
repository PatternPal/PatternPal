using System.Collections.Generic;

namespace IDesign.Tests.TestClasses.StrategyTest2
{
    //this code is from https://refactoring.guru/design-patterns/strategy/csharp/example

    // Concrete Strategies implement the algorithm while following the base
    // Strategy interface. The interface makes them interchangeable in the
    // Context.
    internal class ConcreteStrategyA : IStrategy
    {
        public object DoAlgorithm(object data)
        {
            var list = data as List<string>;
            list.Sort();

            return list;
        }
    }
}
