using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternPal.Core.Checks;

/// <summary>
/// Checks if the parameters of the entity node method are type correct with list of TypeChecks
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
        IParameterized hasParameters = CheckHelper.ConvertNodeElseThrow<IParameterized>(node);

        // Retrieve the parameters the method has to compare to list of TypeChecks and convert to
        // IEntities.
        List< IEntity > methodParameters =
            hasParameters.GetParameters().Select(x => ctx.Graph.Relations.GetEntityByName(x)).ToList();

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
            bool noneCorrect = true;

            // There are no parameters to check with.   
            if (methodParameters.Count == 0)
            {
                // TODO
                ICheckResult temp = new LeafCheckResult
                {
                    Priority = Priority,
                    Correct = false,
                    FeedbackMessage = "There are less parameters than TypeChecks"
                };
                subCheckResultsResults.Add(temp);
                break;
            }

            for (int x = 0; x < methodParameters.Count(); x++)
            {
                var test = methodParameters[x];
                ICheckResult tempCheck;
                tempCheck = typecheck.Check(ctx, methodParameters.ElementAt(x));

                switch (tempCheck)
                {
                    case LeafCheckResult leafCheckResult:
                    {
                        if (leafCheckResult.Correct)
                        {
                            subCheckResultsResults.Add(tempCheck);
                            methodParameters.RemoveAt(x);
                            noneCorrect = false;
                            break;
                        }
                        continue;
                    }
                    case NodeCheckResult nodeCheckResult:
                    {
                        bool anyFalse = false;
                        foreach (LeafCheckResult subcheck in nodeCheckResult.ChildrenCheckResults)
                        {
                            // One of the subchecks was incorrect.
                            if (!subcheck.Correct)
                            {
                                anyFalse = true;
                            }
                        }
                        if (anyFalse)
                        {
                            break;
                        }
                        else
                        {
                            subCheckResultsResults.Add(tempCheck);
                            methodParameters.RemoveAt(x);
                            noneCorrect = false;
                            break;
                        }
                    }
                }

                // reached the last iteration none were correct
                if (x == methodParameters.Count() - 1 && noneCorrect)
                {
                    subCheckResultsResults.Add(tempCheck);
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
