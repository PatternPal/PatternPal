namespace PatternPal.Core.Checks
{
    internal class IfThenOperatorCheck : ICheck
    {
        private readonly List< ICheck > _ifChecks;
        private readonly List< ICheck > _thenChecks;

        public IfThenOperatorCheck(
            List< ICheck > ifChecks,
            List< ICheck > thenChecks)
        {
            _ifChecks = ifChecks;
            _thenChecks = thenChecks;
        }

        CheckResult ICheck.Check(
            RecognizerContext ctx,
            INode entityNode)
        {
            throw new NotImplementedException();
        }
    }
}
