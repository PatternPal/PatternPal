#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using Google.Protobuf.Collections;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

using PatternPal.Protos;

using SyntaxTree.Abstractions.Entities;

//using static PatternPal.CommonResources.ClassFeedbackRes;
using FeedbackType = PatternPal.Protos.FeedbackType;

#endregion

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
                    // TODO CommonResources are not used anymore.
                    if (Score == 100)
                        return null; // CompletionStatusComplete;

                    if (Score >= 80)
                        return null; // CompletionStatusAlmostComplete;

                    return null; // CompletionStatusNotComplete;
                }
                //CurrentMode is mode.StepByStep (this is currently the only other option, so no else if)
                if (Score == 100)
                    return null; // InstructionCompletionStatusComplete;

                if (Score >= 80)
                    return null; // InstructionCompletionStatusAlmostComplete;

                return null; // InstructionCompletionStatusNotComplete;
            }
        }

        /// <summary>
        /// Completion score. 100 means all requirements are fulfilled.
        /// </summary>
        public int Score => Result.Result.Score;

        public bool Expanded { get; set; } = false;

        public List< PatternResultPartViewModel > Children
        {
            get
            {
                List< PatternResultPartViewModel > result = new List< PatternResultPartViewModel >();

                if (IncorrectRequirements.Any())
                {
                    result.Add(
                        new PatternResultPartViewModel(
                            IncorrectRequirements.ToList(),
                            Status.Warning,
                            FeedbackType.FeedbackIncorrect));
                }

                if (CorrectRequirements.Any())
                {
                    result.Add(
                        new PatternResultPartViewModel(
                            CorrectRequirements.ToList(),
                            Status.OK,
                            FeedbackType.FeedbackCorrect));
                }

                return result;
            }
        }

        public SolidColorBrush ProgressBarColor => GetProgressBarColor();

        /// <summary>
        /// IEnumerable that contains checkresults for all correctly implemented requirements
        /// </summary>
        public IEnumerable< object > CorrectRequirements => AddRequirementsFromResults(
            Result.Result.Results,
            new List< CheckResultViewModel >(),
            FeedbackType.FeedbackCorrect);

        /// <summary>
        /// IEnumerable that contains checkresults for all incorrectly implemented requirements
        /// </summary>
        public IEnumerable< object > IncorrectRequirements => AddRequirementsFromResults(
            Result.Result.Results,
            new List< CheckResultViewModel >(),
            FeedbackType.FeedbackIncorrect);

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
                return FeedbackType.FeedbackIncorrect;
            }

            if (Score < 80)
            {
                return FeedbackType.FeedbackSemiCorrect;
            }

            return FeedbackType.FeedbackCorrect;
        }

        /// <summary>
        /// Takes the results and adds the requirements (Group checks) as a list of CheckResultViewModel classes
        /// </summary>
        /// <param name="results"></param>
        /// <param name="destination"></param>
        /// <param name="feedbackType"></param>
        /// <returns>A list that contains CheckResultViewModel classes</returns>
        public IList< CheckResultViewModel > AddRequirementsFromResults(
            RepeatedField< CheckResult > results,
            IList< CheckResultViewModel > destination,
            FeedbackType feedbackType)
        {
            foreach (CheckResult result in results)
            {
                RepeatedField< CheckResult > childFeedback = result.ChildFeedback;

                // Child feedback has at least one element check from the given feedback type
                bool hasChildrenFromGivenFeedbackType =
                    childFeedback.Any(
                        x => x.FeedbackType == feedbackType && !x.ChildFeedback.Any()
                    );

                if (!result.Hidden && hasChildrenFromGivenFeedbackType)
                {
                    destination.Add(
                        new CheckResultViewModel(
                            result,
                            feedbackType));
                }
                else
                    if (result.Hidden
                        && hasChildrenFromGivenFeedbackType
                        && feedbackType == FeedbackType.FeedbackIncorrect)
                    {
                        destination.Add(
                            new CheckResultViewModel(
                                result,
                                feedbackType));
                    }

                if (childFeedback.Any())
                {
                    AddRequirementsFromResults(
                        childFeedback,
                        destination,
                        feedbackType);
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
            CurrentStatus == Status.Warning
                ? KnownMonikers.StatusWarning
                : KnownMonikers.StatusOK;

        public List< object > ChildViewModels { get; set; }

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
                //if (CurrentStatus == Status.Warning)
                //return string.Format(
                //    null /*SummaryTextIncorrectRequirements*/,
                //    ChildrenCount);

                return string.Empty;
                //return string.Format(
                //    null /*SummaryTextCorrectRequirements*/,
                //    ChildrenCount);
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

        public PatternResultPartViewModel(
            List< object > childViewModels,
            Status status,
            FeedbackType feedbackType)
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
        private int CountChildren(
            CheckResult result)
        {
            int totalCount = 0;

            if (result.ChildFeedback.Any())
            {
                foreach (CheckResult childResult in result.ChildFeedback)
                {
                    if (!childResult.ChildFeedback.Any()
                        && childResult.FeedbackType == FeedbackType)
                    {
                        totalCount++;
                    }
                    else
                        if (childResult.Hidden)
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
