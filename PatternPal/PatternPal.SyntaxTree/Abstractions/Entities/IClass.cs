#region

using PatternPal.SyntaxTree.Abstractions.Members;

#endregion

namespace PatternPal.SyntaxTree.Abstractions.Entities
{
    /// <summary>
    /// An <see cref="INode"/> which represents a class.
    /// </summary>
    public interface IClass : IEntity
    {
        /// <summary>
        /// Gets a list of constructors declared in this node.
        /// </summary>
        IEnumerable<IConstructor> GetConstructors();

        /// <summary>
        /// Gets a list of fields declared in this node.
        /// </summary>
        IEnumerable<IField> GetFields();
    }
}
