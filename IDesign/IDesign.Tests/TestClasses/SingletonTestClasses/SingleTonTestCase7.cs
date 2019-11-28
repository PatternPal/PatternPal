using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses
{
   public class SingleTonTestCase7
    {
        private static int testInt;

        private SingleTonTestCase7() { }

        public static int  getInt()
        {
            if (testInt == null)
            {
                testInt = 5;
            }
            return testInt;
        }
    }
}
