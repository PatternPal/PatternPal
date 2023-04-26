using Microsoft.CodeAnalysis.Operations;

namespace PatternPal.Core.Checks;

internal class NotCheck : CheckBase
{
    private readonly ICheck _check;

    internal NotCheck(Priority priority,
        ICheck check) : base(priority)
    {
        _check = check;
    }


    /// <summary>
    /// This method executes all the given checks on the <paramref name="node"/>
    /// In the subCheckResults the instances that fail the test will be stored. Thus the user should want to have an empty childrenCheckResult List.
    /// </summary>
    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        List<ICheckResult> childResults = new List<ICheckResult>();

        ICheckResult checkResult = _check.Check(ctx, node);

        // If all Leafchecks succeed, the entity which should not be present is present.
        bool wrongImplementation = CheckHelper.CheckAllChildrenCorrect(checkResult);

        // In this case we add the result of the check to the childResults, because this shows which entities are present which shouldn't be.
        if (wrongImplementation)
        {
            childResults.Add(checkResult);
        }

        return new NodeCheckResult
        {
            ChildrenCheckResults = childResults,
            Priority = Priority,
            FeedbackMessage = wrongImplementation ? $"Node {node} is a wrong implementation" : $"Node {node} was correctly implemented"
        };
    }
}
