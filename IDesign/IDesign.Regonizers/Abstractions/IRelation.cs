using IDesign.Recognizers.Models;
using SyntaxTree.Abstractions;

namespace IDesign.Recognizers.Abstractions
{
    public interface IRelation : ICheckable
    {
        /// <summary>
        ///     Gets the destination node of this relation
        /// </summary>
        IEntityNode GetDestination();

        /// <summary>
        ///     Gets the type of this relation
        /// </summary>
        RelationType GetRelationType();
    }
}