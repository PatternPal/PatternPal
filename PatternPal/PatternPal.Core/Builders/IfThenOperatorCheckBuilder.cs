using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternPal.Core.Builders
{
    internal class IfThenOperatorCheckBuilder : ICheckBuilder
    {
        private readonly List<ICheckBuilder> _ifCheckBuilders;
        private readonly List<ICheckBuilder> _thenCheckBuilders;

        public IfThenOperatorCheckBuilder(List<ICheckBuilder> ifCheckBuilders, List<ICheckBuilder> thenCheckBuilders)
        {
            _ifCheckBuilders = ifCheckBuilders;
            _thenCheckBuilders = thenCheckBuilders;
        }

        ICheck ICheckBuilder.Build()
        {
            List<ICheck> ifChecks = _ifCheckBuilders.Select(checkBuilder => checkBuilder.Build()).ToList();
            List<ICheck> thenChecks = _thenCheckBuilders.Select(checkBuilder => checkBuilder.Build()).ToList();
            return new IfThenOperatorCheck(ifChecks, thenChecks);
        }
    }
}
