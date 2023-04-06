using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternPal.Core.Checks
{
    internal class IfThenOperatorCheck : ICheck
    {
        private readonly List<ICheck> _ifChecks;
        private readonly List<ICheck> _thenChecks;

        public IfThenOperatorCheck(List<ICheck> ifChecks, List<ICheck> thenChecks)
        {
            _ifChecks = ifChecks;
            _thenChecks = thenChecks;
        }

        bool ICheck.Check(INode entityNode)
        {
            throw new NotImplementedException();
        }
    }
}
