using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses
{
    public class SingleTonTestCase1
    {
        private static SingleTonTestCase1 instance = null;

        private SingleTonTestCase1()
        {

        }

        public static SingleTonTestCase1 Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SingleTonTestCase1();
                }
                return instance;
            }
        }
    }
}
