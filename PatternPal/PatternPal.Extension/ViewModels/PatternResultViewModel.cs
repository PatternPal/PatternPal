#region

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

using PatternPal.Protos;

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
            RecognizeResult result)
        {
            Result = result;
        }

        public RecognizeResult Result { get; }
        public string PatternName => Result.Recognizer.ToString();

        /// <summary>
        /// Text that describes the pattern completeness based on the score value.
        /// Strings are loaded from Resources file
        /// </summary>
        public string PatternCompletionStatusText
        {
            get
            {
                if (ExtensionWindowPackage.CurrentMode == Mode.Default)
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
        public int Score => 100;

        public bool Expanded { get; set; } = false;

        public List< CheckResultViewModel > Children
        {
            get
            {
                List< CheckResultViewModel > results = new List< CheckResultViewModel >();

                foreach (Result result in Result.Results)
                {
                    results.Add(new CheckResultViewModel(result));
                }

                return results;
            }
        }

        public SolidColorBrush ProgressBarColor => GetProgressBarColor();

        /// <summary>
        /// Returns the progress bar color based on the score value.
        /// </summary>
        /// <returns></returns>
        public SolidColorBrush GetProgressBarColor()
        {
            int score = Score;
            return score < 40 ? Brushes.Red : score < 80 ? Brushes.Yellow : Brushes.Green;
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
    }
}
