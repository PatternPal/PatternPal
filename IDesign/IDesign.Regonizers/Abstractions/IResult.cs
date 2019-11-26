using System.Collections.Generic;

namespace IDesign.Recognizers.Abstractions
{
    public interface IResult
    {
        int GetScore();
        IList<ISuggestion> GetSuggestions();
    }
}