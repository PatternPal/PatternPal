﻿using System.Collections.Generic;
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

        public SolidColorBrush GetColor(int score)
        {
            if (score <= 33)
                return Brushes.Red;
            else if (score <= 66)
                return Brushes.Yellow;
            else
                return Brushes.Green;
        }

        public IEnumerable<CheckResultViewModel> Results =>
            Result.Result.GetResults().Select(x => new CheckResultViewModel(x));

        public IEntityNode EntityNode { get; internal set; }
    }
}