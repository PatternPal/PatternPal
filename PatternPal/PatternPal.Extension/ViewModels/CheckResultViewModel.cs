#region

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.ViewModels
{
    /// <summary>
    /// View model for <see cref="Protos.Result"/>s.
    /// </summary>
    public class CheckResultViewModel
    {
        /// <summary>
        /// Icon representing the correctness of this <see cref="Result"/>.
        /// </summary>
        public ImageMoniker Icon
        {
            get
            {
                switch (Result.Correctness)
                {
                    case Result.Types.Correctness.CCorrect:
                        return KnownMonikers.StatusOK;
                    case Result.Types.Correctness.CPartiallyCorrect:
                        return KnownMonikers.StatusAlert;
                    case Result.Types.Correctness.CIncorrect:
                        return KnownMonikers.StatusWarning;
                    default:
                        return KnownMonikers.OnlineStatusUnknown;
                }
            }
        }

        /// <summary>
        /// Requirement to which this <see cref="Result"/> belongs.
        /// </summary>
        public string Requirement { get; }

        /// <summary>
        /// Icon for the matched node.
        /// </summary>
        public ImageMoniker MatchedNodeIcon
        {
            get
            {
                if (Result.MatchedNode == null)
                {
                    return default;
                }

                switch (Result.MatchedNode.Kind)
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
        /// Name of the matched node.
        /// </summary>
        public string MatchedNodeName { get; }

        /// <summary>
        /// The <see cref="Protos.Result"/> this view model represents.
        /// </summary>
        public Result Result { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="CheckResultViewModel"/> class.
        /// </summary>
        /// <param name="result">The <see cref="Protos.Result"/> this view model is for.</param>
        public CheckResultViewModel(
            Result result)
        {
            Requirement = result.Requirement;
            MatchedNodeName = result.MatchedNode?.Name;

            Result = result;
        }
    }
}
