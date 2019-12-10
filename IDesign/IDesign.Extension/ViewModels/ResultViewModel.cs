using System.Collections.Generic;
using System.Linq;
using IDesign.Core.Models;
using IDesign.Recognizers.Abstractions;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

namespace IDesign.Extension.ViewModels
{
    public class ClassViewModel
    {
        public ClassViewModel(IEntityNode entityNode)
        {
            EntityNode = entityNode;
        }

        public IEntityNode EntityNode { get; set; }
        public string ClassName => EntityNode.GetName();

        public ResultViewModel BestMatch => Results.OrderByDescending(x => x.Score).FirstOrDefault();
        public List<ResultViewModel> Results { get; set; } = new List<ResultViewModel>();
    }

    public class ResultViewModel
    {
        public ResultViewModel(RecognitionResult result)
        {
            Result = result;
        }

        public RecognitionResult Result { get; set; }
        public string PatternName => Result.Pattern.Name;
        public int Score => Result.Result.GetScore();

        public IEnumerable<CheckResultViewModel> Results =>
            Result.Result.GetResults().Select(x => new CheckResultViewModel(x));
    }

    public class CheckResultViewModel
    {
        public CheckResultViewModel(ICheckResult result)
        {
            Result = result;
        }

        public ICheckResult Result { get; set; }
        public string Message => Result.GetMessage();
        public int Score => Result.GetScore();
        public FeedbackType Type => Result.GetFeedbackType();

        public ImageMoniker Icon
        {
            get
            {
                switch (Type)
                {
                    case FeedbackType.Correct:
                        return KnownMonikers.StatusOK;
                    case FeedbackType.SemiCorrect:
                        return KnownMonikers.StatusExcluded;
                    case FeedbackType.Incorrect:
                        return KnownMonikers.StatusError;
                }

                return KnownMonikers.StatusHelp;
            }
        }

        public IEnumerable<CheckResultViewModel> SubResults =>
            Result.GetChildFeedback().Select(x => new CheckResultViewModel(x));
    }
}