using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace PatternPal.Core.Checks;
internal class InterfaceCheck : CheckBase
{
    private readonly IEnumerable<ICheck> _checks;

    internal InterfaceCheck(Priority priority,
        IEnumerable<ICheck> checks) : base(priority)
    {
        _checks = checks;
    }

    public override ICheckResult Check(
         RecognizerContext ctx,
         INode node)
    {
        IInterface interfaceEntity = CheckHelper.ConvertNodeElseThrow<IInterface>(node);


        bool hasFailedSubCheck = false;
        IList<ICheckResult> subCheckResults = new List<ICheckResult>();
        foreach(ICheck check in _checks)
        {
            ICheckResult checkResult;
            switch(check)
            {
                case ModifierCheck modifierCheck:
                    {
                        checkResult = modifierCheck.Check(ctx, interfaceEntity);
                        break;
                    }
                case MethodCheck methodCheck:
                    {

                        break;
                    }
                case FieldCheck fieldCheck:
                    {
                        break;
                    }
                case ConstructorCheck constructorCheck:
                    {
                        break;
                    }
                case NotCheck notCheck:
                    {
                        break;
                    }
                case UsesCheck usesCheck:
                    {
                        break;
                    }
                default:
                    {
                        throw CheckHelper.InvalidSubCheck(this, check);
                    }
            }

            subCheckResults.Add(checkResult);
            if (!checkResult.Correctness)
                hasFailedSubCheck = true;
        }
        return new NodeCheckResult
        {
            ChildrenCheckResults = subCheckResults,
            Correctness = !hasFailedSubCheck,
            FeedbackMessage = $"Found class '{interfaceEntity}'",
            Priority = Priority,
        };
    }
}
