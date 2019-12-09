using IDesign.Recognizers.Models;

namespace IDesign.Recognizers.Abstractions
{
    public interface ICheck<T> where T : class, ICheckable
    {
        ICheckResult Check(T elementToCheck);
    }
}
