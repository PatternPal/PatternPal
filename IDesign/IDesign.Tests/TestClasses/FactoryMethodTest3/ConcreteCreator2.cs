using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.FactoryMethodTest3
{
    class ConcreteCreator2 : Creator
    {
        public override IProduct FactoryMethod()
        {
            return new ConcreteProduct2();
        }
    }
}
