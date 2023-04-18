namespace PatternPal.Core.Builders;

internal class IfThenOperatorCheckBuilder : CheckBuilderBase
{
    private readonly List<ICheckBuilder> _ifCheckBuilders;
    private readonly List<ICheckBuilder> _thenCheckBuilders;
    
    public IfThenOperatorCheckBuilder(Priority priority, List<ICheckBuilder> ifCheckBuilders, List<ICheckBuilder> thenCheckBuilders) : base(priority)
    {
        _ifCheckBuilders = ifCheckBuilders;
        _thenCheckBuilders = thenCheckBuilders;
    }

    public override ICheck Build()
    {
        List<ICheck> ifChecks = _ifCheckBuilders.Select(checkBuilder => checkBuilder.Build()).ToList();
        List<ICheck> thenChecks = _thenCheckBuilders.Select(checkBuilder => checkBuilder.Build()).ToList();
        return new IfThenOperatorCheck(Priority, ifChecks, thenChecks);
    }
}
