#region

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using PatternPal.Protos;

using static PatternPal.Protos.CheckResult.Types;

#endregion

namespace PatternPal.Extension.ViewModels
{
    public class ResultViewModel
    {
        public ResultViewModel(
            RecognizeResult result)
        {
            Result = result;
        }

        public RecognizeResult Result { get; set; }
        public string PatternName => Result.Recognizer.ToString();
        public int Score => (int)Result.Score;

        public SolidColorBrush Color => GetColor((int)Result.Score);

        public IEnumerable< CheckResultViewModel > Results =>
            Result.Results.Select(
                x => new CheckResultViewModel(
                    x,
                    GetFeedbackType()));

        public SolidColorBrush GetColor(
            int score)
        {
            return score < 40 ? Brushes.Red : score < 80 ? Brushes.Yellow : Brushes.Green;
        }

        public FeedbackType GetFeedbackType()
        {
            uint score = Result.Score;
            if (score < 40)
            {
                return FeedbackType.FeedbackIncorrect;
            }

            if (score < 80)
            {
                return FeedbackType.FeedbackSemiCorrect;
            }

            return FeedbackType.FeedbackCorrect;
        }
    }
}
