using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Recognizers.Abstractions
{
    public interface IResult
    {
        int GetScore();
        IList<ISuggestion> GetSuggestions();
    }
}
