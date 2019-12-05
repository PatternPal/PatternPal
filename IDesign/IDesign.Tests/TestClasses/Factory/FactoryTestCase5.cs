using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.Factory
{
    public class FactoryTestCase5
    {
        public FactoryTestCase5() { }

        public IProduct Create()
        {
            return new ProductB();
        }
    }
}
