using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.FactoryTestClasses
{
   public class FactoryTestCase3
    {
        public FactoryTestCase3() { }
        private ProductA product = new ProductA();
        public IProduct CreateProductA()
        {
            return product;
        }
    }
}
