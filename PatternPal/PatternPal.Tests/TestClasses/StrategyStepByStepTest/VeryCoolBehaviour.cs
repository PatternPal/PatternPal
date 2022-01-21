using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternPal.Tests.TestClasses.StrategyStepByStepTest
{
    public class VeryCoolBehaviour : IBehaviour
    {
        public string Thing()
        {
            return "yay!";
        }
    }
}
