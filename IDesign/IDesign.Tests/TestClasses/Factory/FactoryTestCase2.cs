using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.FactoryTestClasses
{
    public class FactoryTestCase2
    {
        public FactoryTestCase2() { }

        public ProductA CreateProductA()
        {
            var product = new ProductA();
            return product;
        }
    }
}
