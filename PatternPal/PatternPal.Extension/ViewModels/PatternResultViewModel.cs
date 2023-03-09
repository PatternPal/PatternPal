using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using PatternPal.Core.Models;
using PatternPal.Recognizers.Abstractions;
using SyntaxTree.Abstractions.Entities;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

using PatternPal.Protos;

using static PatternPal.CommonResources.ClassFeedbackRes;

namespace PatternPal.Extension.ViewModels
{
    /// <summary>
    /// Enum that is used to determine the correct icon to display
    /// </summary>
    public enum Status
    {
        Warning,
        OK
    }

    public class PatternResultViewModel
    {
        public PatternResultViewModel(RecognitionResult result)
        {
            //Result = result;
        }

        public PatternResultViewModel(
            RecognizerResult result)
        {
            Result = result;
        }

        public RecognizerResult Result { get; }
        public string PatternName => Result.DetectedPattern;

        /// <summary>
        /// Text that describes the pattern completeness based on the score value.
        /// Strings are loaded from Resources file
        /// </summary>
        public string PatternCompletionStatusText
        {
            get
            {
                if (PatternPalExtensionPackage.CurrentMode == Mode.Default)
                {
                    if (Score == 100)
                        return CompletionStatusComplete;

                    if (Score >= 80)
                        return CompletionStatusAlmostComplete;

                    return CompletionStatusNotComplete;
                }
                //CurrentMode is mode.StepByStep (this is currently the only other option, so no else if)
                if (Score == 100)
                    return InstructionCompletionStatusComplete;

                if (Score >= 80)
                    return InstructionCompletionStatusAlmostComplete;

                return InstructionCompletionStatusNotComplete;
            }
        }

        /// <summary>
        /// Completion score. 100 means all requirements are fulfilled.
        /// </summary>
        public int Score => (int)Result.Score;

        public bool Expanded { get; set; } = false;

        public List<PatternResultPartViewModel> Children
        {
            get
            {
                List< PatternResultPartViewModel > result = new List<PatternResultPartViewModel>();

                if (IncorrectRequirements.Any())
                {
                    result.Add(new PatternResultPartViewModel(IncorrectRequirements.ToList(), Status.Warning, FeedbackType.Incorrect));
                }

                if (CorrectRequirements.Any())
                {
                    result.Add(new PatternResultPartViewModel(CorrectRequirements.ToList(), Status.OK, FeedbackType.Correct));
                }

                return result;
            }
        }

        public SolidColorBrush ProgressBarColor => GetProgressBarColor();

        /// <summary>
        /// IEnumerable that contains checkresults for all correctly implemented requirements
        /// </summary>
        public IEnumerable< object > CorrectRequirements => Array.Empty< object >( );
            //AddRequirementsFromResults(Result.Result.GetResults(), new List<CheckResultViewModel>(), FeedbackType.Correct);

        /// <summary>
        /// IEnumerable that contains checkresults for all incorrectly implemented requirements
        /// </summary>
        public IEnumerable<object> IncorrectRequirements =>Array.Empty< object >( );
            //AddRequirementsFromResults(Result.Result.GetResults(), new List<CheckResultViewModel>(), FeedbackType.Incorrect);

        public IEntity EntityNode { get; internal set; }

        /// <summary>
        /// Returns the progress bar color based on the score value.
        /// </summary>
        /// <returns></returns>
        public SolidColorBrush GetProgressBarColor()
        {
            int score = Score;
            return score < 40 ? Brushes.Red : score < 80 ? Brushes.Yellow : Brushes.Green;
        }

        /// <summary>
        /// Returns the correct feedback type based on the score
        /// </summary>
        /// <returns></returns>
        public FeedbackType GetFeedbackType()
        {
            if (Score < 40)
            {
                return FeedbackType.Incorrect;
            }

            if (Score < 80)
            {
                return FeedbackType.SemiCorrect;
            }

            return FeedbackType.Correct;
        }

        /// <summary>
        /// Takes the results and adds the requirements (Group checks) as a list of CheckResultViewModel classes
        /// </summary>
        /// <param name="results"></param>
        /// <param name="destination"></param>
        /// <param name="feedbackType"></param>
        /// <returns>A list that contains CheckResultViewModel classes</returns>
        public IList<CheckResultViewModel> AddRequirementsFromResults(
            IEnumerable<ICheckResult> results,
            IList<CheckResultViewModel> destination,
            FeedbackType feedbackType
        )
        {
            foreach (ICheckResult result in results)
            {
                IEnumerable< ICheckResult > childFeedback = result.GetChildFeedback();

                // Child feedback has at least one element check from the given feedback type
                bool hasChildrenFromGivenFeedbackType = 
                    childFeedback.Any(
                        x => x.GetFeedbackType() == feedbackType && 
                            !x.GetChildFeedback().Any()
                    );

                if (!result.IsHidden && hasChildrenFromGivenFeedbackType)
                {
                    destination.Add(new CheckResultViewModel(result, feedbackType));
                }
                else if (result.IsHidden && hasChildrenFromGivenFeedbackType && feedbackType == FeedbackType.Incorrect)
                {
                    destination.Add(new CheckResultViewModel(result, feedbackType));
                }

                if (childFeedback.Any())
                {
                    AddRequirementsFromResults(childFeedback, destination, feedbackType);
                }
            }

            return destination;
        }
    }

    public class PatternResultPartViewModel
    {
        /// <summary>
        /// The icon to be shown for the result part.
        /// </summary>
        public ImageMoniker Icon =>
            CurrentStatus == Status.Warning ? KnownMonikers.StatusWarning : KnownMonikers.StatusOK;

        public List<object> ChildViewModels { get; set; }

        public Status CurrentStatus { get; set; }

        public FeedbackType FeedbackType { get; set; }

        /// <summary>
        /// Text that contains the amount of correct/incorrect requirements.
        /// Part of the string is loaded from the Resources file.
        /// </summary>
        public string SummaryText
        {
            get
            {
                if (CurrentStatus == Status.Warning)
                    return string.Format(SummaryTextIncorrectRequirements, ChildrenCount);

                return string.Format(SummaryTextCorrectRequirements, ChildrenCount);
            }
        }

        public int ChildrenCount
        {
            get
            {
                int count = 0;

                foreach (CheckResultViewModel model in ChildViewModels)
                {
                    count += CountChildren(model.Result);
                }

                return count;
            }
        }

        public PatternResultPartViewModel(List<object> childViewModels, Status status, FeedbackType feedbackType)
        {
            ChildViewModels = childViewModels;
            CurrentStatus = status;
            FeedbackType = feedbackType;
        }

        /// <summary>
        /// Counts the children recursively for a given ICheckResult
        /// </summary>
        /// <param name="result">The ICheckResult for which to count the children</param>
        /// <returns>Children count</returns>
        private int CountChildren(ICheckResult result)
        {
            int totalCount = 0;

            if (result.GetChildFeedback().Any())
            {
                foreach (ICheckResult childResult in result.GetChildFeedback())
                {
                    if (!childResult.GetChildFeedback().Any() && childResult.GetFeedbackType() == FeedbackType)
                    {
                        totalCount++;
                    }
                    else if (childResult.IsHidden)
                    {
                        totalCount += CountChildren(childResult);
                    }
                }

                return totalCount;
            }

            return totalCount;
        }
    }
}
