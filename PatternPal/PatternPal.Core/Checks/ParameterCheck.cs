using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternPal.Core.Checks;

/// <summary>
/// Checks the parameters of the entity node method 
/// </summary>
internal class ParameterCheck : CheckBase
{
    // The types the parameters should have.
    private readonly IEnumerable< TypeCheck > _parameterTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterCheck"/> class.
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="parameterTypes">A list of types the node method parameters should have.</param>
    internal ParameterCheck(
        Priority priority,
        IEnumerable< TypeCheck > parameterTypes)
        : base(priority)
    {
        _parameterTypes = parameterTypes;
    }

    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        IMethod method = CheckHelper.ConvertNodeElseThrow<IMethod>(node);

        // Retrieve the parameters the method has to compare to list of TypeChecks and convert to
        // IEntities.
        List< IEntity > methodParameters =
            method.GetParameters().Select(x => ctx.Graph.Relations.GetEntityByName(x)).ToList();

        List<ICheckResult> subCheckResultsResults = new List<ICheckResult>();

        // Method has no parameters
        if (!methodParameters.Any())
        {
            return new NodeCheckResult
            {
                ChildrenCheckResults = subCheckResultsResults,
                FeedbackMessage = $"The method has no parameters.",
                Priority = Priority
            };
        }

        // For each typecheck, check whether one of the parameters of method has the correct type
        // and if so remove from method parameters list.
        foreach (TypeCheck typecheck in _parameterTypes)
        {
            for (int x = 0; x < methodParameters.Count(); x++)
            {
                ICheckResult tempCheck = typecheck.Check(ctx, methodParameters.ElementAt(x));
                subCheckResultsResults.Add( tempCheck );

                switch (tempCheck)
                {
                    case LeafCheckResult leafCheckResult:
                    {
                        if (leafCheckResult.Correct)
                        {
                            methodParameters.RemoveAt(x);
                        }
                        break;
                    }
                    case NodeCheckResult nodeCheckResult:
                    {
                        bool anyCorrect = true;
                        foreach (LeafCheckResult subcheck in nodeCheckResult.ChildrenCheckResults)
                        {
                            // One of the subchecks was incorrect.
                            if (!subcheck.Correct)
                            {
                                anyCorrect = false;
                            }
                        }
                        if (!anyCorrect)
                        {
                            break;
                        }
                        else
                        {
                            methodParameters.RemoveAt(x);
                            break;
                        }
                    }
                }
            }
        }
        // TODO feedback message
        return new NodeCheckResult
        {
            ChildrenCheckResults = subCheckResultsResults, 
            FeedbackMessage = String.Empty, 
            Priority = Priority
        };
    }
}
