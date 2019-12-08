using System.Collections.Generic;
using IDesign.Recognizers.Abstractions;

namespace IDesign.Recognizers.Models.Output
{
    public class Result : IResult
    {
        public List<IFeedback> Suggestions { get; set; } = new List<IFeedback>();
        public int Score { get; set; }

        public int GetScore()
        {
            return Score;
        }

        public IList<IFeedback> GetSuggestions()
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