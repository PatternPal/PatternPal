using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;

namespace IDesign.Recognizers.Models.Output
{
    public class Result : IResult
    {
        public List<ICheckResult> Results { get; set; } = new List<ICheckResult>();

        public Dictionary<IEntityNode, string> RelatedSubTypes = new Dictionary<IEntityNode, string>();

        public int GetScore()
        {
            var total = (float)Results.Sum(x => x.GetTotalChecks());
            var green = Results.Sum(x => x.GetScore());
            return total <= 0 ? 0 : (int)(green / total * 100f);
        }

        public IList<ICheckResult> GetResults()
        {
            return Results;
        }

        public IDictionary<IEntityNode, string> GetRelatedSubTypes()
        {
            return RelatedSubTypes;
        }
    }
}
