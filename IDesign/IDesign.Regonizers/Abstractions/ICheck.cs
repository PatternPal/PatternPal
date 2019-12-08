using IDesign.Recognizers.Models;

namespace IDesign.Recognizers.Abstractions
{
    public interface ICheck<T> where T : ICheckable
    {
        IFeedback Check(T elementToCheck);
    }
}
