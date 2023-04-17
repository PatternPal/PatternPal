namespace PatternPal.Core.Checks;

internal class ParameterCheck : CheckBase
{
    private readonly List<TypeCheck> _parameterTypes;

    public ParameterCheck(Priority priority,
        List<TypeCheck> parameterTypes) : base(priority)
    { 
        _parameterTypes = parameterTypes;
    }
    public override ICheckResult Check(RecognizerContext ctx, INode node)
    {
        throw new NotImplementedException();
    }
}
