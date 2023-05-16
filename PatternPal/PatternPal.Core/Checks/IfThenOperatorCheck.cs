﻿namespace PatternPal.Core.Checks
{
    internal class IfThenOperatorCheck : CheckBase
    {
        private readonly List< ICheck > _ifChecks;
        private readonly List< ICheck > _thenChecks;

        public override int DependencyCount => throw new NotImplementedException();

        public IfThenOperatorCheck(
            Priority priority,
            List< ICheck > ifChecks,
            List< ICheck > thenChecks)
            : base(priority)
        {
            _ifChecks = ifChecks;
            _thenChecks = thenChecks;
        }

        public override ICheckResult Check(
            IRecognizerContext ctx,
            INode entityNode)
        {
            throw new NotImplementedException();
        }
    }
}
