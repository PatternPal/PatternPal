using System;

namespace PatternPal.Tests.TestClasses.StrategyTest3
{
    //code is from https://www.dofactory.com/net/strategy-design-pattern

    /// <summary>
    ///     A 'ConcreteStrategy' class
    /// </summary>
    internal class ConcreteStrategyC : Strategy
    {
        public override void AlgorithmInterface()
        {
            Console.WriteLine(
                "Called ConcreteStrategyC.AlgorithmInterface()");
        }
    }
}
