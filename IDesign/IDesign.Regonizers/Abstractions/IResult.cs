using System.Collections.Generic;
using SyntaxTree.Abstractions.Entities;

namespace IDesign.Recognizers.Abstractions
{
    public interface IResult
    {
        /// <summary>
        ///     Get the score of this result in percentage.
        ///     On a scale from 0 to 100
        /// </summary>
        /// <returns>A score in percentage</returns>
        int GetScore();

        /// <summary>
        ///     Get a list of suggestions for this result.
        /// </summary>
        /// <returns>A list of suggestions</returns>
        IList<ICheckResult> GetResults();

        IDictionary<IEntity, string> GetRelatedSubTypes();
    }
}
