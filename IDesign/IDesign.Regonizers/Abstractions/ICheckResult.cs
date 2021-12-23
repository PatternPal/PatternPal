using System.Collections.Generic;
using SyntaxTree.Abstractions;

namespace IDesign.Recognizers.Abstractions
{
    public enum FeedbackType
    {
        Correct,
        SemiCorrect,
        Incorrect
    }

    public enum CheckType
    {
        Optional,
        KnockOut
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
        INode GetElement();

        /// <summary>
        ///     Get the feedback message
        /// </summary>
        /// <returns>The feedback message</returns>
        IResourceMessage GetFeedback();

        /// <summary>
        ///     Changes the score and total checks
        /// </summary>
        void ChangeScore(float score);

        /// <summary>
        ///     Whether the Element or Group has an incorrect knock out check
        /// </summary>
        /// <returns>True is one or more checks are knock-out checks and are incorrect, false otherwise</returns>
        bool HasIncorrectKnockOutCheck { get; set; }
    }
}
