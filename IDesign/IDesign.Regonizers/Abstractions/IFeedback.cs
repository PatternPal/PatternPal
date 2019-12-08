using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace IDesign.Recognizers.Abstractions
{

    public enum FeedbackType
    {
        Correct,
        SemiCorrect,
        Incorrect
    }

    public interface IFeedback
    {
        /// <summary>
        ///     Get the message for this feedback
        /// </summary>
        /// <returns>The suggestion message</returns>
        string GetMessage();


        /// <summary>
        ///     Get the message for this feedback
        /// </summary>
        /// <returns>The suggestion message</returns>
        IEnumerable<IFeedback> GetChildFeedback();

        /// <summary>
        ///     Get the score in percentage
        /// </summary>
        /// <returns>The suggestion message</returns>
        int GetScore();

        /// <summary>
        ///     Get the type of this feedback
        /// </summary>
        /// <returns>The suggestion message</returns>
        FeedbackType GetFeedbackType();

        /// <summary>
        ///     Get the syntax node related to this feedback
        /// </summary>
        /// <returns>The related syntax node</returns>
        SyntaxNode GetSyntaxNode();
    }
}