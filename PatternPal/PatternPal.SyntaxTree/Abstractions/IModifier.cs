namespace PatternPal.SyntaxTree.Abstractions
{
    /// <summary>
    /// Represents a modifier of an <see cref="INode"/>.
    /// </summary>
    /// <example>Examples are private, static, and abstract</example>
    public interface IModifier
    {
        string GetName();
    }
}
