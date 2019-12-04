using Microsoft.CodeAnalysis;

namespace IDesign.Recognizers.Abstractions
{
    public interface ISuggestion
    {
        /// <summary>
        ///     Get the suggestion message for this suggestion
        /// </summary>
        /// <returns>The suggestion message</returns>
        string GetMessage();

        /// <summary>
        ///     Get the syntax node for this suggestion
        /// </summary>
        /// <returns>The related syntax node</returns>
        SyntaxNode GetSyntaxNode();
    }
}