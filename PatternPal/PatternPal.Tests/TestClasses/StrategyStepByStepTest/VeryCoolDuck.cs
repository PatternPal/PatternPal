using System.Linq;
using System.Text;

namespace PatternPal.Tests.TestClasses.StrategyStepByStepTest
{
    public class VeryCoolDuck : Duck
    {
        public VeryCoolDuck()
        {
            this.Behaviour = new VeryCoolBehaviour();
        }
    }
}
