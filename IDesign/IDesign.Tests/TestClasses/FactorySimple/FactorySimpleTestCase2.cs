using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.FactorySimple
{
    public class FactorySimpleTestCase2
    {
        private ProductA product = new ProductA();

        public ProductA Create()
        {
            return product;
        }
    }
}
