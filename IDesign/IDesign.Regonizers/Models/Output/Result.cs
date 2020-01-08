using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;

namespace IDesign.Recognizers.Models.Output
{
    public class Result : IResult
    {
        public List<ICheckResult> Results { get; set; } = new List<ICheckResult>();

        public int GetScore()
        {
            var total = (float) Results.Sum(x => x.GetTotalChecks());
            var green = Results.Sum(x => x.GetScore());
            if (total <= 0)
            {
                return 0;
            }
            return (int)(green / total * 100f);
        }

        public IList<ICheckResult> GetResults() => Results;

        public override string ToString()
        {
            var res = "";
            res += GetScore();

            foreach (var suggestie in Results)
            {
                res += ", " + suggestie.GetMessage();
            }

            return res;
        }
    }
}