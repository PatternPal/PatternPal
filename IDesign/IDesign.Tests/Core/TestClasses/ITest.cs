using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.Core.TestClasses
{
    interface ITest
    {
        int x { get; set; }
        int y { get; set; }
        int Sum();
    }
}
