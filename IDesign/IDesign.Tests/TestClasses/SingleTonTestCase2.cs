using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses
{
    public class SingleTonTestCase2
    {
        public static readonly SingleTonTestCase2 _obj = new SingleTonTestCase2();
        SingleTonTestCase2() { }
    }
}
