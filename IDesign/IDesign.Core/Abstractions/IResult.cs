using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Core
{
    public interface IResult
    {
        int GetScore();
        List<ISuggestion> GetSuggestions();
    }
}
