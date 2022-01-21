using System;
using System.Collections.Generic;
using System.Text;

namespace PatternPal.Tests.TestClasses
{
    public class SingleTonTestCase6
    {
        private static SingleTonTestCase6 _instance;

        private SingleTonTestCase6() { }

        public static SingleTonTestCase6 GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SingleTonTestCase6();
            }
            return _instance;
        }
    }
}


