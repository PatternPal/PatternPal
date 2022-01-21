using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDesign.Tests.TestClasses.StrategyStepByStepTest
{
    public class VeryCoolBehaviour : IBehaviour
    {
        public string Thing()
        {
            return "yay!";
        }
    }
}
