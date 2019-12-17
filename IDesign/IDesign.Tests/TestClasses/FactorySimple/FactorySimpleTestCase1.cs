using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.FactorySimple
{
   public class FactorySimpleTestCase1
    {
        public ProductA Create()
        {
            return new ProductA();
        }
    }
}
