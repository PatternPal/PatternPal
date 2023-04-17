using System.Collections.Generic;
using System.Linq;
using System.Text;

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
