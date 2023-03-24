using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using PatternPal.Protos;

namespace PatternPal.Extension.ViewModels
{
    public class ResultViewModel
    {
        public ResultViewModel(RecognizerResult result)
        {
            Result = result;
        }

        public RecognizerResult Result { get; set; }
        public string PatternName => Result.DetectedPattern;
        public int Score => Result.Result.Score;

        public SolidColorBrush Color => GetColor(Result.Result.Score);

        public IEnumerable<CheckResultViewModel> Results =>
            Result.Result.Results.Select(x => new CheckResultViewModel(x, GetFeedbackType()));

        public SolidColorBrush GetColor(int score)
        {
            return score < 40 ? Brushes.Red : score < 80 ? Brushes.Yellow : Brushes.Green;
        }

        public FeedbackType GetFeedbackType()
        {
            var score = Result.Result.Score;
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
