using SyntaxTree.Abstractions.Entities;

namespace IDesign.Recognizers.Abstractions
{
    public interface IRecognizer
    {
        /// <summary>
        ///     Analyses the given node for this pattern
        /// </summary>
        /// <param name="node">Entity node what it should check</param>
        /// <returns>The result object</returns>
        IResult Recognize(IEntity node);
    }
}
