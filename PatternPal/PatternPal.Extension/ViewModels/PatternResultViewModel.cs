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

        public List<EntityCheckResultViewModel> EntityChecks
        {
            get
            {
                return Result.EntityResults.Select(result => new EntityCheckResultViewModel(result)).ToList();
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

    public class EntityCheckResultViewModel
    {
        public EntityResult EntityResult;
        public bool Expanded { get; set; }
        public string Name => EntityResult.Name;

        public string MatchedNodeName { get; }

        public ImageMoniker MatchedNodeIcon
        {
            get
            {
                if (EntityResult.MatchedNode == null)
                {
                    return default;
                }

                switch (EntityResult.MatchedNode.Kind)
                {
                    case MatchedNode.Types.MatchedNodeKind.MnkClass:
                        return KnownMonikers.Class;
                    case MatchedNode.Types.MatchedNodeKind.MnkInterface:
                        return KnownMonikers.Interface;
                    case MatchedNode.Types.MatchedNodeKind.MnkConstructor:
                    case MatchedNode.Types.MatchedNodeKind.MnkMethod:
                        return KnownMonikers.Method;
                    case MatchedNode.Types.MatchedNodeKind.MnkField:
                        return KnownMonikers.Field;
                    case MatchedNode.Types.MatchedNodeKind.MnkProperty:
                        return KnownMonikers.Property;
                    default:
                        return default;
                }
            }
        }

        /// <summary>
        /// Completion score. 100 means all requirements are fulfilled.
        /// </summary>
        public int Score => 100;

        public EntityCheckResultViewModel(EntityResult entityResult)
        {
            EntityResult = entityResult;
            MatchedNodeName = entityResult.MatchedNode?.Name;
        }

        public List<CheckResultViewModel> Children
        {
            get
            {
                List<CheckResultViewModel> results = new List<CheckResultViewModel>();

                foreach (Result result in EntityResult.Requirements)
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
