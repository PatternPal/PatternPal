using System.Collections.Generic;

namespace IDesign.Tests.TestClasses.StrategyFactoryMethodTest1
{
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
