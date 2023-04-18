namespace PatternPal.Core.Builders;

internal class ParameterCheckBuilder : CheckBuilderBase
{
    private readonly List<TypeCheck> _parameterTypes;

    internal ParameterCheckBuilder(Priority priority,
        IEnumerable<TypeCheck> parameterTypes) : base(priority)
    { 
        _parameterTypes = new List<TypeCheck>(parameterTypes);
    }

    public override ICheck Build() => new ParameterCheck(Priority, _parameterTypes);
}
