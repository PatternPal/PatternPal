﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses
{
    class TestClass1
    {
        public TestClass1(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x { get; set; }
        public int y { get; set; }

        public int Sum()
        {
            return x + y;
        }
    }
}
