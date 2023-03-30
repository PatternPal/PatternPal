using SyntaxTree.Abstractions.Entities;

namespace SyntaxTree.Abstractions
{
    public interface IRelation<N> where N : INode
    {
        /// <summary>
        ///     Gets the destination node of this relation
        /// </summary>
        N GetDestination();

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
