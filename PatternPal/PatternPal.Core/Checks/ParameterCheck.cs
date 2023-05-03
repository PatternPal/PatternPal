using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternPal.Core.Checks;

/// <summary>
/// Checks if the parameters of an <see cref="IParameterized"/> are in accorance with the types in <see cref="_parameterTypes"/>.
/// </summary>
internal class ParameterCheck : CheckBase
{
    // Parameters of the provided INode should have types corresponding with this list of TypeChecks.
    private readonly IEnumerable< TypeCheck > _parameterTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterCheck"/> class.
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="parameterTypes">A list of types the node parameters should have.</param>
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

        // Retrieve the parameters the node has to compare to list of TypeChecks and convert to
        // IEntities.
        List< IEntity > nodeParameters =
            hasParameters.GetParameters().Select(x => ctx.Graph.Relations.GetEntityByName(x)).ToList();

        List<ICheckResult> subCheckResultsResults = new List<ICheckResult>();

        // Node has no parameters
        if (!nodeParameters.Any())
        {
            return new NodeCheckResult
            {
                ChildrenCheckResults = subCheckResultsResults,
                FeedbackMessage = $"The method has no parameters.",
                Priority = Priority
            };
        }
        // No TypeChecks were provided
        if (!_parameterTypes.Any())
        {
            return new NodeCheckResult
            {
                ChildrenCheckResults = subCheckResultsResults,
                FeedbackMessage = $"No TypeChecks were provided.",
                Priority = Priority
            };
        }

        // For each typecheck, check whether one of the parameters of node has the correct type
        // and if so remove from node parameters list.
        foreach (TypeCheck typecheck in _parameterTypes)
        {
            bool noneCorrect = true;

            // There are no parameters to check with.   
            if (nodeParameters.Count == 0)
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

            for (int x = 0; x < nodeParameters.Count(); x++)
            {
                var test = nodeParameters[x];
                ICheckResult tempCheck;
                tempCheck = typecheck.Check(ctx, nodeParameters.ElementAt(x));

                switch (tempCheck)
                {
                    case LeafCheckResult leafCheckResult:
                    {
                        if (leafCheckResult.Correct)
                        {
                            subCheckResultsResults.Add(tempCheck);
                            nodeParameters.RemoveAt(x);
                            noneCorrect = false;
                            break;
                        }
                        continue;
                    }
                    case NodeCheckResult nodeCheckResult:
                    {
                        bool anyTrue = false;
                        foreach (LeafCheckResult subcheck in nodeCheckResult.ChildrenCheckResults)
                        {
                            // One of the subchecks was correct.
                            if (subcheck.Correct)
                            {
                                anyTrue = true;
                            }
                        }
                        if (anyTrue)
                        {
                            subCheckResultsResults.Add(tempCheck);
                            nodeParameters.RemoveAt(x);
                            noneCorrect = false;
                            break; 
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                // reached the last iteration none were correct
                if (x == nodeParameters.Count() - 1 && noneCorrect)
                {
                    subCheckResultsResults.Add(tempCheck);
                }
            }
        }

        return new NodeCheckResult
        {
            ChildrenCheckResults = subCheckResultsResults, 
            FeedbackMessage = $"Found parameters for following node: {node}.", 
            Priority = Priority
        };
    }
}
