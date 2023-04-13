namespace PatternPal.Core.Builders;
internal class TypeCheckBuilder : ICheckBuilder
{
    //used to get the type
    private readonly Func<INode> _getNode;

    internal TypeCheckBuilder(
        Func<INode> getNode)
    {
        _getNode = getNode;
    }

    public ICheck Build() => new TypeCheck(_getNode);
}
