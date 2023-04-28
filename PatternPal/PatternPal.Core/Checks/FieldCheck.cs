using Microsoft.CodeAnalysis.VisualBasic;
using PatternPal.Recognizers.Models.Checks;

namespace PatternPal.Core.Checks;

/// <summary>
/// An instance in which you can execute <see cref="ModifierCheck"/>s and <see cref="TypeCheck"/>s via a list of <see cref="_checks"/> on a field.
/// </summary>
internal class FieldCheck : CheckBase
{
    private readonly IEnumerable<ICheck> _checks;

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldCheck"/> class. 
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="checks">A list of subchecks that should be tested</param>
    internal FieldCheck(Priority priority, 
        IEnumerable<ICheck> checks) : base(priority)
    {
        _checks = checks;
    }

    /// <summary>
    /// This method executes all the given <see cref="ModifierCheck"/>s and <see cref="TypeCheck"/>s on the <paramref name="node"/>
    /// </summary>
    public override ICheckResult Check(RecognizerContext ctx, INode node)
    {
        IField fieldEntity= CheckHelper.ConvertNodeElseThrow<IField>(node);
        IList<ICheckResult> subCheckResults = new List<ICheckResult>();
        
        foreach (ICheck check in _checks)
        {
            ICheckResult checkResult;
            switch (check)
            {
                case ModifierCheck modifierCheck:
                    {
                        checkResult = modifierCheck.Check(ctx, fieldEntity);
                        break;
                    }
                case TypeCheck typeCheck:
                    {
                        checkResult = typeCheck.Check(ctx, fieldEntity);
                        break;
                    }
                default:
                    throw CheckHelper.InvalidSubCheck(this, check);
            }
            subCheckResults.Add(checkResult);
        }
        return new NodeCheckResult
        {
            ChildrenCheckResults = subCheckResults,
            FeedbackMessage = $"Found field '{fieldEntity}'",
            Priority = Priority
        };
    }
}
