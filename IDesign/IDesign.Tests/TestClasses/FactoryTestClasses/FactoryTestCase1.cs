using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.FactoryTestClasses
{
    public class FactoryTestCase1
    {
        public FactoryTestCase1() { }
        public IProduct CreateProductA()
        {
            var product = new ProductA();
            return product;
        }
    }
}
