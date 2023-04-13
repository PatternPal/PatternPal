namespace PatternPal.Core.Checks;

internal class ParameterCheck : ICheck
{
    private readonly List<TypeCheck> _parameterTypes;

    public ParameterCheck(
        List<TypeCheck> parameterTypes)
    { 
        _parameterTypes = parameterTypes;
    }
    public bool Check(RecognizerContext ctx, INode node)
    {
        throw new NotImplementedException();
    }
}
