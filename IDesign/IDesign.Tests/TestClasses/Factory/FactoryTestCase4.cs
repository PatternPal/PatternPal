using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.Factory
{
   public  class FactoryTestCase4
    {
        public FactoryTestCase4() { }

        public ProductA Create()
        {
            return new ProductB();
        }
    }
}
