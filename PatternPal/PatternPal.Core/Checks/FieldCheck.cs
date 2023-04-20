using Microsoft.CodeAnalysis.VisualBasic;
using PatternPal.Recognizers.Models.Checks;

namespace PatternPal.Core.Checks;

/// <summary>
/// An instance in which you can execute <see cref="ModifierCheck"/>s and <see cref="TypeCheck"/>s via a list of <see cref="_checks"/> on a field.
/// Results will be stored in <see cref="_childrenCheckResults"/>
/// </summary>
internal class FieldCheck : CheckBase
{
    private readonly IEnumerable<ICheck> _checks;
    private List<ICheckResult> _childrenCheckResults;

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldCheck"/> class. 
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="checks">A list of subchecks that should be tested</param>
    internal FieldCheck(Priority priority, 
        IEnumerable<ICheck> checks) : base(priority)
    {
        _checks = checks;
        _childrenCheckResults = new();
    }

    /// <summary>
    /// This method executes all the given <see cref="ModifierCheck"/>s and <see cref="TypeCheck"/>s on the <paramref name="node"/>
    /// </summary>
    public override ICheckResult Check(RecognizerContext ctx, INode node)
    {
        IField field = CheckHelper.ConvertNodeElseThrow<IField>(node);
        
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
                    throw CheckHelper.InvalidSubCheck(this, check);
            }
            _childrenCheckResults.Add(checkResult);
        }
        return new NodeCheckResult { ChildrenCheckResults = _childrenCheckResults,
                                     FeedbackMessage = $"Found field: {field}",
                                     Priority = Priority };
    }
}
