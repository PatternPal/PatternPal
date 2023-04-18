using Microsoft.CodeAnalysis.VisualBasic;
using PatternPal.Recognizers.Models.Checks;

namespace PatternPal.Core.Checks;

internal class FieldCheck : CheckBase
{
    private readonly IEnumerable<ICheck> _checks;
    private List<ICheckResult> _childrenCheckResults;
    internal FieldCheck(Priority priority, 
        IEnumerable<ICheck> checks) : base(priority)
    {
        _checks = checks;
        _childrenCheckResults = new();
    }

    public override ICheckResult Check(RecognizerContext ctx, INode node)
    {
        if (node is not IField field)
        {
            throw new NotImplementedException("Field check was incorrect");
        }

        bool correctness = true;
        string feedbackMessage = "Field checks succeeded.";
        foreach (ICheck check in _checks)
        {
            ICheckResult checkResult;
            switch (check)
            {
                case ModifierCheck modifierCheck:
                    {
                        checkResult = modifierCheck.Check(ctx, field);
                        break;
                    }
                case TypeCheck typeCheck:
                    {
                        checkResult = typeCheck.Check(ctx, field);
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException($"Unexpected check: {check.GetType().Name}");
                    }
            }
            if (!checkResult.Correctness)
            {
                feedbackMessage = "At least one of the field checks failed";
                correctness = false;
            }
                _childrenCheckResults.Add(checkResult);
        }
        return new NodeCheckResult { ChildrenCheckResults = _childrenCheckResults,
                                     Correctness = correctness,
                                     FeedbackMessage = feedbackMessage,
                                     Priority = Priority.Undefined };
    }
}
