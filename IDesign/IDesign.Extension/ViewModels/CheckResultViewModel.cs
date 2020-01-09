using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

namespace IDesign.Extension.ViewModels
{
    public class CheckResultViewModel
    {
        public CheckResultViewModel(ICheckResult result)
        {
            Result = result;
        }

        public ICheckResult Result { get; set; }
        public string Message => Result.GetMessage();
        public float Score => Result.GetScore();
        public FeedbackType Type => Result.GetFeedbackType();

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