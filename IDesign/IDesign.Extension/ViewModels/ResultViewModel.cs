using System.Collections.Generic;
using System.Linq;
using IDesign.Core;
using IDesign.Core.Models;
using IDesign.Recognizers.Abstractions;

namespace IDesign.Extension.ViewModels
{
    public class ClassViewModel
    {
        public ClassViewModel(IEntityNode entityNode)
        {
            EntityNode = entityNode;
        }

        public IEntityNode EntityNode { get; set; }
        public string ClassName => EntityNode.GetName();

        public ResultViewModel BestMatch => Results.OrderByDescending(x => x.Score).FirstOrDefault();
        public List<ResultViewModel> Results { get; set; } = new List<ResultViewModel>();
    }

    public class ResultViewModel
    {
        public ResultViewModel(RecognitionResult result)
        {
            Result = result;
        }

        public RecognitionResult Result { get; set; }
        public string PatternName => Result.Pattern.Name;
        public int Score => Result.Result.GetScore();
        public List<SuggestionViewModel> Suggestions { get; set; } = new List<SuggestionViewModel>();
    }

    public class SuggestionViewModel
    {
        public SuggestionViewModel(IFeedback feedback, IEntityNode node)
        {
            Feedback = feedback;
            Node = node;
        }

        public IFeedback Feedback { get; set; }
        public IEntityNode Node { get; set; }
        public string SuggestionText => Feedback.GetMessage();
    }
}