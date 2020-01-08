using IDesign.Recognizers.Models;

namespace IDesign.Recognizers.Abstractions
{
    /// <summary>
    ///     An interface for a check that could be evaluated on an element
    /// </summary>
    /// <typeparam name="T">Type of the element to be checked</typeparam>
    public interface ICheck<T> where T : class, ICheckable
    {
        /// <summary>
        ///     Evaluates the check for the given element
        /// </summary>
        /// <param name="elementToCheck">The element to evaluate the check on</param>
        /// <returns>A result of the evaluation</returns>
        ICheckResult Check(T elementToCheck);
    }
}