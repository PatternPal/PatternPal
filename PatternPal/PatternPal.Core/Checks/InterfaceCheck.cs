using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using PatternPal.Protos;

namespace PatternPal.Core.Checks;
/// <summary>
/// An instance in which you can execute multiple checks via a list <see cref="_checks"/> on an interface.
/// </summary>
internal class InterfaceCheck : CheckBase
{
    private readonly IEnumerable<ICheck> _checks;

    /// <summary>
    /// Initializes a new instance of the <see cref="InterfaceCheck"/> class. 
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="checks">A list of subchecks that should be tested</param>
    internal InterfaceCheck(Priority priority,
        IEnumerable<ICheck> checks) : base(priority)
    {
        _checks = checks;
    }

    /// <summary>
    /// This method executes all the given checks on the <paramref name="node"/>
    /// </summary>
    public override ICheckResult Check(
         RecognizerContext ctx,
         INode node)
    {
        IInterface interfaceEntity = CheckHelper.ConvertNodeElseThrow<IInterface>(node);

        IList<ICheckResult> subCheckResults = new List<ICheckResult>();
        foreach (ICheck check in _checks)
        {
            switch (check)
            {
                case ModifierCheck modifierCheck:
                    {
                        subCheckResults.Add(modifierCheck.Check(ctx, interfaceEntity));
                        break;
                    }
                case MethodCheck methodCheck:
                    {
                        foreach (IMethod method in interfaceEntity.GetMethods())
                        {
                            subCheckResults.Add(methodCheck.Check(ctx, method));
                        }
                        break;
                    }
                case NotCheck notCheck:
                    {
                        subCheckResults.Add(notCheck.Check(ctx, interfaceEntity));
                        break;
                    }
                case UsesCheck usesCheck:
                    {
                        subCheckResults.Add(usesCheck.Check(ctx, interfaceEntity));
                        break;
                    }
                default:
                    {
                        throw CheckHelper.InvalidSubCheck(this, check);
                    }
            }
        }
        return new NodeCheckResult
        {
            ChildrenCheckResults = subCheckResults,
            FeedbackMessage = $"Found class '{interfaceEntity}'",
            Priority = Priority,
        };
    }
}
