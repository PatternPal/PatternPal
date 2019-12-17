using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.FactoryMethodTest1
{
    //Context class
    class DubbleFactory : BeerFactory
    {
        public override IBeer BrewBier()
        {
            return new Dubbel();
        }
    }
}
