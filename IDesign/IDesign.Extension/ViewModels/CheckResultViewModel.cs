using System.Collections.Generic;
using System.Linq;
using IDesign.CommonResources;
using IDesign.Recognizers.Abstractions;

namespace IDesign.Extension.ViewModels
{
    public class CheckResultViewModel
    {
        public ICheckResult Result { get; set; }
        public string Message { get; set; }
        public float Score => Result.GetScore();
        public FeedbackType ChildrenFeedbackType { get; set; }
        public IEnumerable<CheckResultViewModel> SubResults => GetSubResults();

        public CheckResultViewModel(ICheckResult result, FeedbackType childrenFeedbackType)
        {
            Result = result;
            Message = ResourceUtils.ResultToString(Result);
            ChildrenFeedbackType = childrenFeedbackType;
        }

        public CheckResultViewModel(ICheckResult result) : this(result, FeedbackType.Correct) { }

        private IEnumerable<CheckResultViewModel> GetSubResults()
        {
            var toReturn = new List<CheckResultViewModel>();

            foreach (ICheckResult result in Result.GetChildFeedback())
            {
                GetSubResultsRecursive(toReturn, result);
            }

            return toReturn;
        }

        private void GetSubResultsRecursive(List<CheckResultViewModel> toReturn, ICheckResult result)
        {
            if (result.IsHidden)
            {
                foreach (var sub in result.GetChildFeedback())
                {
                    GetSubResultsRecursive(toReturn, sub);
                }
            }
            else if (!result.GetChildFeedback().Any() && result.GetFeedbackType() == ChildrenFeedbackType)
            {
                toReturn.Add(new CheckResultViewModel(result));
            }
        }
    }
}
