using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using IDesign.CommonResources;

namespace IDesign.Extension.ViewModels
{
    public class CheckResultViewModel
    {
        public CheckResultViewModel(ICheckResult result)
        {
            Result = result;
            Message = ResourceUtils.ResultToString(Result);
            Type = Result.GetFeedbackType();
        }

        public CheckResultViewModel(ICheckResult result, FeedbackType feedbackType)
        {
            Result = result;
            Type = feedbackType;
        }

        public ICheckResult Result { get; set; }
        public string Message { get; set; }
        public float Score => Result.GetScore();
        public FeedbackType type;
        public FeedbackType Type 
        {
            get
            {
                return type;
            }
            set
            {
                type = value; 
            }
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

        public IEnumerable<CheckResultViewModel> SubResults =>
            Result.GetChildFeedback().Select(x => new CheckResultViewModel(x));
    }
}
