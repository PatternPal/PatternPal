using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.FactorySimple
{
    public class FactorySimpleTestCase4
    {
        private FactorySimpleTestCase2 factory;
        public ProductA Create()
        {
            var product = new ProductA();
            factory = new FactorySimpleTestCase2();
            return product;
        }
    }
}
