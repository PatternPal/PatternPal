using System.Collections.Generic;
using IDesign.Recognizers.Abstractions;

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

        public override string ToString()
        {
            var res = "";

            res += Score;

            foreach (var suggestie in Suggestions)
                res += ", " + suggestie.GetMessage();

            return res;
        }
    }
}