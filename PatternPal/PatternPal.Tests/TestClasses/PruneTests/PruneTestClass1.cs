using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternPal.Tests.TestClasses.Strategy;

namespace PatternPal.Tests.TestClasses.PruneTests
{
    internal class PruneTestClass1
    {
        public static int CalculateClient()
        {
            return 1 + 1;
        }
    }

    internal class ClassUsesPruneClass 
    {
        public int pizza = PruneTestClass1.CalculateClient();
    }
}
