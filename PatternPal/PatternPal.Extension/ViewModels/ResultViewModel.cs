#region

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using PatternPal.Protos;

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
        public int Score => 100;

        public SolidColorBrush Color => GetColor(100);

        public IEnumerable< CheckResultViewModel > Results => Enumerable.Empty< CheckResultViewModel >();
            //Result.Results.Select(
            //    x => new CheckResultViewModel(
            //        x,
            //        GetFeedbackType()));

        public SolidColorBrush GetColor(
            int score)
        {
            return score < 40 ? Brushes.Red : score < 80 ? Brushes.Yellow : Brushes.Green;
        }
    }
}
