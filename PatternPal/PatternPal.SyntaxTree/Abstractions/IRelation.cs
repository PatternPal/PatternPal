using SyntaxTree.Abstractions.Entities;

namespace SyntaxTree.Abstractions
{
    public interface IRelation : INode
    {
        /// <summary>
        ///     Gets the destination node of this relation
        /// </summary>
        IEntity GetDestination();

        /// <summary>
        ///     Gets the type of this relation
        /// </summary>
        RelationType GetRelationType();
    }

    public enum RelationType
    {
        Implements,
        ImplementedBy,
        Extends,
        ExtendedBy,
        Uses,
        UsedBy,
        Creates,
        CreatedBy
    }
}
