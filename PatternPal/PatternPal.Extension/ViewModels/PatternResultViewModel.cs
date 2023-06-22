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
    /// <summary>
    /// View model for a <see cref="RecognizeResult"/>.
    /// </summary>
    public class PatternResultViewModel
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PatternResultViewModel"/> class.
        /// </summary>
        /// <param name="result">The <see cref="RecognizeResult"/> this view model is for.</param>
        public PatternResultViewModel(
            RecognizeResult result)
        {
            Result = result;
        }

        /// <summary>
        /// The <see cref="RecognizeResult"/> this view model is for.
        /// </summary>
        public RecognizeResult Result { get; }

        /// <summary>
        /// The name of the <see cref="Recognizer"/> to which the result belongs.
        /// </summary>
        public string PatternName => Result.Recognizer.ToString();

        /// <summary>
        /// Completion score. 100 means all requirements are fulfilled.
        /// </summary>
        public int Score => 100;

        /// <summary>
        /// Whether this view model is currently expanded.
        /// </summary>
        public bool Expanded { get; set; }

        /// <summary>
        /// The top level results of this <see cref="Result"/>.
        /// </summary>
        public List< EntityCheckResultViewModel > EntityChecks => Result.EntityResults.Select(result => new EntityCheckResultViewModel(result)).ToList();

        /// <summary>
        /// The color of the progress bar.
        /// </summary>
        public SolidColorBrush ProgressBarColor => Score < 40 ? Brushes.Red : Score < 80 ? Brushes.Yellow : Brushes.Green;
    }

    /// <summary>
    /// View model for a <see cref="Protos.EntityResult"/>.
    /// </summary>
    public class EntityCheckResultViewModel
    {
        /// <summary>
        /// The <see cref="Protos.EntityResult"/> this view model represents.
        /// </summary>
        public readonly EntityResult EntityResult;

        /// <summary>
        /// Whether this view model is currently expanded.
        /// </summary>
        public bool Expanded { get; set; }

        /// <summary>
        /// The description of this <see cref="EntityResult"/>.
        /// </summary>
        public string Name => EntityResult.Name;

        /// <summary>
        /// The name of the matched node.
        /// </summary>
        public string MatchedNodeName { get; }

        /// <summary>
        /// Icon for the matched node.
        /// </summary>
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

        /// <summary>
        /// Creates a new instance of the <see cref="EntityCheckResultViewModel"/> class.
        /// </summary>
        /// <param name="entityResult">The <see cref="Protos.EntityResult"/> this view model is for.</param>
        public EntityCheckResultViewModel(
            EntityResult entityResult)
        {
            EntityResult = entityResult;
            MatchedNodeName = entityResult.MatchedNode?.Name;
        }

        /// <summary>
        /// The view models of the child results of this <see cref="EntityResult"/>.
        /// </summary>
        public List< CheckResultViewModel > Children
        {
            get
            {
                List< CheckResultViewModel > results = new List< CheckResultViewModel >();

                foreach (Result result in EntityResult.Requirements)
                {
                    results.Add(new CheckResultViewModel(result));
                }

                return results;
            }
        }

        /// <summary>
        /// The color of the progress bar.
        /// </summary>
        public SolidColorBrush ProgressBarColor => Score < 40 ? Brushes.Red : Score < 80 ? Brushes.Yellow : Brushes.Green;
    }
}
