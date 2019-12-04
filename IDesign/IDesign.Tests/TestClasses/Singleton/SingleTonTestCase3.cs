using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses
{
    public class SingleTonTestCase3
    {
        private static SingleTonTestCase3 instance = new SingleTonTestCase3();
        public static SingleTonTestCase3 Instance => instance;
    }
}
