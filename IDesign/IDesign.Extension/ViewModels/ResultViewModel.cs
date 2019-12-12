using System.Collections.Generic;
using System.Linq;
using IDesign.Core.Models;

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

        public IEnumerable<CheckResultViewModel> Results =>
            Result.Result.GetResults().Select(x => new CheckResultViewModel(x));
    }
}