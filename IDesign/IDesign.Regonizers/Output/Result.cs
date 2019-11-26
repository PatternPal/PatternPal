using IDesign.Recognizers.Abstractions;
using System.Collections.Generic;

namespace IDesign.Recognizers.Output
{
    public class Result : IResult
    {
        public List<ISuggestion> Suggestions { get; set; } = new List<ISuggestion>();
        public int Score { get; set; }

        public int GetScore()
        {
            return Score;
        }

        public IList<ISuggestion> GetSuggestions()
        {
            return Suggestions;
        }
    }
}