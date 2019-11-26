using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses
{
    public class SingleTonTestCase5
    {
        private SingleTonTestCase5() { }

        private static SingleTonTestCase5 _instance;

        public static SingleTonTestCase5 GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SingleTonTestCase5();
            }
            return _instance;
        }
    }

    public class SingleTonTestCase6
    {
        private static SingleTonTestCase6 _instance;

        private SingleTonTestCase6()
        {

        }
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
