using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.FactorySimple
{
    public class FactorySimpleTestCase3
    {
        private FactorySimpleTestCase1 Create()
        {
            var factory = new FactorySimpleTestCase1();
            return factory;
        }
    }
}
