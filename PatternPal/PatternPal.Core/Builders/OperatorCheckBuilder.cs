namespace PatternPal.Core.Builders;

enum TypeOperatorCheck
{
    All,
    Any
}


internal class OperatorCheckBuilder : ICheckBuilder
{
    private readonly TypeOperatorCheck _operatorCheck;
    private readonly List<ICheckBuilder> _childCheckBuilders;

    public OperatorCheckBuilder(TypeOperatorCheck typeOperatorCheck)
    {
        _operatorCheck = typeOperatorCheck;
        _childCheckBuilders = new List<ICheckBuilder>();
    }

    ICheck ICheckBuilder.Build()
    {
        List<ICheck> childChecks = _childCheckBuilders.Select(checkBuilder => checkBuilder.Build()).ToList();
        return new OperatorCheck(_operatorCheck, childChecks);
    }

    internal OperatorCheckBuilder AddCheckToOperator(
        params ICheckBuilder[] childCheckBuilders)
    {
        _childCheckBuilders.AddRange(childCheckBuilders);
        return this;
    }
}
