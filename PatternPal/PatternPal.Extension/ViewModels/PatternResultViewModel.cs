#region

using System.Collections.Generic;
using System.Windows.Media;

using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.ViewModels
{
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
        /// Completion score. 100 means all requirements are fulfilled.
        /// </summary>
        public int Score => 100;

        public bool Expanded { get; set; }

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
}
