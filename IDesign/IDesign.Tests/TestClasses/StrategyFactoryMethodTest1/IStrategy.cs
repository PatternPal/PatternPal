using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StrategyFactoryMethodTest1
{
   public interface IStrategy
    {
        object DoAlgorithm(object data);
    }
}
