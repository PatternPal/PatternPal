using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using IDesign.CommonResources;
using IDesign.Core.Models;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.Output;
using Microsoft.VisualStudio.ProjectSystem.VS;

namespace IDesign.Extension.ViewModels
{
    public class PatternResultViewModel
    {
        public PatternResultViewModel(RecognitionResult result)
        {
            Result = result;
        }

        public RecognitionResult Result { get; set; }
        public string PatternName => Result.Pattern.Name;
        public int Score => Result.Result.GetScore();

        public List<Named> Childs => new List<Named>()
        {
            new PatternResultPartViewModel("Components ", Components.ToList()),
            new PatternResultPartViewModel("Improvements ", Improvements.ToList()),
            new PatternResultPartViewModel("All requirements ", Requirements.ToList()),
        };

        public SolidColorBrush Color => GetColor(Result.Result.GetScore());

        public SolidColorBrush GetColor(int score)
        {
            return score < 40 ? Brushes.Red : score < 80 ? Brushes.Yellow : Brushes.Green;
        }

        public FeedbackType GetFeedbackType()
        {
            int score = Result.Result.GetScore();
            if (score < 40)
                return FeedbackType.Incorrect;
            if (score < 80)
                return FeedbackType.SemiCorrect;
            return FeedbackType.Correct;
        }

        public IEnumerable<object> Components
        {
            get
            {
                var dict = new Dictionary<string, PatternComponentViewModel>();

                foreach (var relatedSubType in Result.Result.GetRelatedSubTypes())
                {
                    if (!dict.ContainsKey(relatedSubType.Value))
                    {
                        var key = relatedSubType.Value;
                        var name = ResourceUtils.ResourceMessageToString(new ResourceMessage(key));
                        dict.Add(relatedSubType.Value, new PatternComponentViewModel(name));
                    }
                    dict[relatedSubType.Value].EntityNodes.Add(relatedSubType.Key);
                }

                return dict.Values;
            }
        }

        public IEnumerable<object> Requirements
        {
            get { return Result.Result.GetResults().Select(x => new CheckResultViewModel(x)); }
        }

        public IEnumerable<object> Improvements
        {
            get { return AddImprovementsFromResults(Result.Result.GetResults(), new List<CheckResultViewModel>()); }
        }

        public IList<CheckResultViewModel> AddImprovementsFromResults(IEnumerable<ICheckResult> results, IList<CheckResultViewModel> destination)
        {
            foreach (var result in results)
            {

                var childFeedback = result.GetChildFeedback();
                if (childFeedback.Any())
                    AddImprovementsFromResults(childFeedback, destination);

                else if (result.GetFeedbackType() == FeedbackType.Incorrect)
                    destination.Add(new CheckResultViewModel(result));

            }
            return destination;
        }

        public IEntityNode EntityNode { get; internal set; }
    }

    public interface Named
    {
        string Name { get; }
    }

    public class PatternResultPartViewModel : Named
    {
        public string Name { get; set; }
        public List<object> ChildViewModels { get; set; }

        public PatternResultPartViewModel(string name, List<object> childViewModels)
        {
            Name = name;
            ChildViewModels = childViewModels;
        }
    }
}