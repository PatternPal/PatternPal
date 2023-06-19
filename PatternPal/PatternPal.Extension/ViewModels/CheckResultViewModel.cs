#region

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.ViewModels
{
    public class CheckResultViewModel
    {
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

        public string Requirement { get; }

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

        public string MatchedNodeName { get; }

        public Result Result { get; }

        public CheckResultViewModel(
            Result result)
        {
            Requirement = result.Requirement;
            MatchedNodeName = result.MatchedNode?.Name;

            Result = result;
        }
    }
}
