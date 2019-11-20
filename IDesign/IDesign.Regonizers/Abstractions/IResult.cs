using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Regonizers.Abstractions
{
    public interface IResult
    {
        int GetScore();
        List<ISuggestion> GetSuggestions();
    }
}
