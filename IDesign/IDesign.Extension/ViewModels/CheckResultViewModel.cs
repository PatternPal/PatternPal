using System.Collections.Generic;
using System.Linq;
using IDesign.CommonResources;
using IDesign.Recognizers.Abstractions;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

namespace IDesign.Extension.ViewModels
{
    public class CheckResultViewModel
    {
        public FeedbackType type;

        public CheckResultViewModel(ICheckResult result)
        {
            Result = result;
            Message = ResourceUtils.ResultToString(Result);
            Type = Result.GetFeedbackType();
        }

        public CheckResultViewModel(ICheckResult result, FeedbackType feedbackType)
        {
            Result = result;
            Message = ResourceUtils.ResultToString(Result);
            Type = feedbackType;
        }

        public ICheckResult Result { get; set; }
        public string Message { get; set; }
        public float Score => Result.GetScore();

        public FeedbackType Type
        {
            get => type;
            set => type = value;
        }

        public ImageMoniker Icon
        {
            get
            {
                switch (Type)
                {
                    case FeedbackType.Correct:
                        return KnownMonikers.OnlineStatusAvailable;
                    case FeedbackType.SemiCorrect:
                        return KnownMonikers.OnlineStatusAway;
                    case FeedbackType.Incorrect:
                        return KnownMonikers.OnlineStatusBusy;
                }

                return KnownMonikers.StatusHelp;
            }
        }

        public IEnumerable<CheckResultViewModel> SubResults => GetSubResults();

        private IEnumerable<CheckResultViewModel> GetSubResults()
        {
            var toReturn = new List<CheckResultViewModel>();

            foreach (ICheckResult result in Result.GetChildFeedback())
            {
                RecursiveFunction(toReturn, result);
            }

            return toReturn;
        }

        private void RecursiveFunction(List<CheckResultViewModel> toReturn, ICheckResult result)
        {
            if (result.IsHidden)
            {
                foreach (var sub in result.GetChildFeedback())
                {
                    RecursiveFunction(toReturn, sub);
                }
            }
            else
            {
                toReturn.Add(new CheckResultViewModel(result));
            }
        }
    }
}
