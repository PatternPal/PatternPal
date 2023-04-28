namespace PatternPal.Core.Checks;

internal class ParameterCheck : CheckBase
{
    private readonly IEnumerable< TypeCheck > _parameterTypes;

    internal ParameterCheck(
        Priority priority,
        IEnumerable< TypeCheck > parameterTypes)
        : base(priority)
    {
        _parameterTypes = parameterTypes;
    }

    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        throw new NotImplementedException();
    }
}
