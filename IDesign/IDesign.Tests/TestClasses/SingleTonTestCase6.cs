using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses
{
    public class SingleTonTestCase6
    {
        public static SingleTonTestCase6 _instance;

        public SingleTonTestCase6() { }

        public  SingleTonTestCase6 Instance()
        {
            if (_instance == null)
            {
                _instance = new SingleTonTestCase6();
            }
            return _instance;
        }
    }
}
