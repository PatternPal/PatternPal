using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StrategyTest3
{
    /// <summary>

    /// A 'ConcreteStrategy' class

    /// </summary>

    class ConcreteStrategyC : Strategy

    {
        public override void AlgorithmInterface()
        {
            Console.WriteLine(
              "Called ConcreteStrategyC.AlgorithmInterface()");
        }
    }
}
