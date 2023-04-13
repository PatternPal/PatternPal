namespace PatternPal.Core.Builders;

internal class ParameterCheckBuilder : ICheckBuilder
{
    private readonly List<TypeCheck> _parameterTypes;

    internal ParameterCheckBuilder(
        IEnumerable<TypeCheck> parameterTypes)
    { 
        _parameterTypes = new List<TypeCheck>(parameterTypes);
    }

    public ICheck Build() => new ParameterCheck(_parameterTypes);
}
