#region

using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.ViewModels
{
    public class CheckResultViewModel
    {
        public string Requirement { get; }
        public string MatchedNodeName { get; }

        public CheckResultViewModel(
            Result result)
        {
            Requirement = result.Requirement;
            MatchedNodeName = result.MatchedNode?.Name;
        }
    }
}
