using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using IDesign.Core.Models;
using IDesign.Recognizers.Abstractions;

namespace IDesign.Extension.ViewModels
{
    public class ResultViewModel
    {
        public ResultViewModel(RecognitionResult result)
        {
            Result = result;
        }

        public RecognitionResult Result { get; set; }
        public string PatternName => Result.Pattern.Name;
        public int Score => Result.Result.GetScore();

        public SolidColorBrush Color => GetColor(Result.Result.GetScore());

        public IEnumerable<CheckResultViewModel> Results =>
            Result.Result.GetResults().Select(x => new CheckResultViewModel(x, GetFeedbackType()));

        public IEntityNode EntityNode { get; internal set; }

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
    }
}
