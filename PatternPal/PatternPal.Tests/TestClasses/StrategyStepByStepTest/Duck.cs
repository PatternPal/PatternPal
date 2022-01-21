using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternPal.Tests.TestClasses.StrategyStepByStepTest
{
    public abstract class Duck
    {
        public IBehaviour Behaviour { get; set; }
        public void PerformBehaviour()
        {
            Console.WriteLine(Behaviour.Thing());
        }
    }
}
