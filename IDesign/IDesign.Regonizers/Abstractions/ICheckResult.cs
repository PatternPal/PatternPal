using System.Collections.Generic;
using IDesign.Recognizers.Models;

namespace IDesign.Recognizers.Abstractions
{
    public enum FeedbackType
    {
        Correct,
        SemiCorrect,
        Incorrect
    }
    public interface ICheckResult
    {
        /// <summary>
        ///     Get the message for this feedback
        /// </summary>
        /// <returns>The suggestion message</returns>
        IEnumerable<ICheckResult> GetChildFeedback();

        /// <summary>
        ///     Get the score in percentage
        /// </summary>
        /// <returns>The suggestion message</returns>
        float GetScore();


        /// <summary>
        ///     Get the score in percentage
        /// </summary>
        /// <returns>The suggestion message</returns>
        float GetTotalChecks();

        /// <summary>
        ///     Get the type of this feedback
        /// </summary>
        /// <returns>The suggestion message</returns>
        FeedbackType GetFeedbackType();

        /// <summary>
        ///     Get the syntax node related to this feedback
        /// </summary>
        /// <returns>The related syntax node</returns>
        ICheckable GetElement();

        IResourceMessage GetFeedback();

        /// <summary>
        ///     Changes the score and total checks
        /// </summary>
        /// <returns>The related syntax node</returns>
        void ChangeScore(float score);
    }
}
