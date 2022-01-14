using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using IDesign.Core.Models;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.ElementChecks;
using SyntaxTree.Abstractions.Entities;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using SyntaxTree.Abstractions;
using static IDesign.CommonResources.ClassFeedbackRes;

namespace IDesign.Extension.ViewModels
{
    public enum Status
    {
        Warning,
        OK
    }

    public class PatternResultViewModel
    {
        public PatternResultViewModel(RecognitionResult result)
        {
            Result = result;
        }

        public RecognitionResult Result { get; set; }
        public string PatternName => Result.Pattern.Name;

        public string PatternCompletionStatusText
        {
            get
            {
                if (Score == 100)
                    return CompletionStatusComplete;
                
                if (Score >= 80)
                    return CompletionStatusAlmostComplete;

                return CompletionStatusNotComplete;
            }
        }
        public int Score => Result.Result.GetScore();

        public List<PatternResultPartViewModel> Children
        {
            get
            {
                var result = new List<PatternResultPartViewModel>();
                if (IncorrectRequirements.Any())
                {
                    result.Add(new PatternResultPartViewModel(IncorrectRequirements.ToList(), Status.Warning));
                }

                if (CorrectRequirements.Any())
                {
                    result.Add(new PatternResultPartViewModel(CorrectRequirements.ToList(), Status.OK));
                }

                return result;
            }
        }

        public SolidColorBrush Color => GetColor(Result.Result.GetScore());

        public IEnumerable<object> CorrectRequirements =>
            AddRequirementsFromResults(Result.Result.GetResults(), new List<CheckResultViewModel>(), FeedbackType.Correct);

        public IEnumerable<object> IncorrectRequirements =>
            AddRequirementsFromResults(Result.Result.GetResults(), new List<CheckResultViewModel>(), FeedbackType.Incorrect);

        public IEntity EntityNode { get; internal set; }

        public SolidColorBrush GetColor(int score)
        {
            return score < 40 ? Brushes.Red : score < 80 ? Brushes.Yellow : Brushes.Green;
        }

        public FeedbackType GetFeedbackType()
        {
            var score = Result.Result.GetScore();
            if (score < 40)
            {
                return FeedbackType.Incorrect;
            }

            if (score < 80)
            {
                return FeedbackType.SemiCorrect;
            }

            return FeedbackType.Correct;
        }

        public IList<CheckResultViewModel> AddRequirementsFromResults(
            IEnumerable<ICheckResult> results,
            IList<CheckResultViewModel> destination,
            FeedbackType feedbackType
        )
        {
            foreach (var result in results)
            {
                var childFeedback = result.GetChildFeedback();
                if (childFeedback.Any())
                    AddRequirementsFromResults(childFeedback, destination, feedbackType);
                
                else if (result.GetFeedbackType() == feedbackType)
                    destination.Add(new CheckResultViewModel(result));
            }
            return destination;
        }
    }


    public class PatternResultPartViewModel
    {
        public ImageMoniker Icon =>
            CurrentStatus == Status.Warning ? KnownMonikers.StatusWarning : KnownMonikers.StatusOK;

        public List<object> ChildViewModels { get; set; }

        public int ChildrenCount
        {
            get
            {
                int count = 0;

                foreach (CheckResultViewModel model in ChildViewModels)
                {
                    count += CountChildrenRecursive(0, model.Result);
                }

                return count;
            }
        }
        public Status CurrentStatus { get; set; }

        public string SummaryText
        {
            get
            {
                if (CurrentStatus == Status.Warning)
                    return string.Format(SummaryTextIncorrectRequirements, ChildrenCount);

                return string.Format(SummaryTextCorrectRequirements, ChildrenCount);
            }
        }

        public PatternResultPartViewModel(List<object> childViewModels, Status status)
        {
            ChildViewModels = childViewModels;
            CurrentStatus = status;
        }

        private int CountChildrenRecursive(int count, ICheckResult result)
        {
            int countTest = 0;

            if (result.GetChildFeedback().Any())
            {
                foreach (var sub in result.GetChildFeedback())
                {
                    countTest += CountChildrenRecursive(count, sub);
                }

                return countTest;
            }

            return ++count;
        }
    }
}
