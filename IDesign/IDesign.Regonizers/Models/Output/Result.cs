using System.Collections.Generic;
using IDesign.Recognizers.Abstractions;

namespace IDesign.Recognizers.Models.Output
{
    public class Result : IResult
    {
        public List<ICheckResult> Results { get; set; } = new List<ICheckResult>();
        public int Score { get; set; }

        public int GetScore()
        {
            return Score;
        }

        public IList<ICheckResult> GetResults()
        {
            return Results;
        }

        public override string ToString()
        {
            var res = "";
            res += Score;

            foreach (var suggestie in Results)
                res += ", " + suggestie.GetMessage();

            return res;
        }
    }
}