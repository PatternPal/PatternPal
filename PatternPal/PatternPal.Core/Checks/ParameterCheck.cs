namespace PatternPal.Core.Checks;

/// <summary>
/// Checks if the parameters of an <see cref="IParameterized"/> are in accordance with the types in <see cref="_parameterTypes"/>.
/// </summary>
internal class ParameterCheck : CheckBase
{
    // Parameters of the provided INode should have types corresponding with this list of TypeChecks.
    private readonly IEnumerable< TypeCheck > _parameterTypes;

    // The dependency count, declared as nullable so we can check whether we have calculated it
    // already.
    private int ? _dependencyCount;

    /// <summary>
    /// As a <see cref="TypeCheck"/> is a dependency to another <see cref="INode"/>, all <see cref="TypeCheck"/>s
    /// in <see cref="_parameterTypes"/> are dependencies.
    /// </summary>
    public override int DependencyCount
    {
        get
        {
            _dependencyCount ??= _parameterTypes.Count();
            return _dependencyCount.Value;
        }
    }

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

    /// <inheritdoc />
    public override ICheckResult Check(
        IRecognizerContext ctx,
        INode node)
    {
        IParameterized hasParameters = CheckHelper.ConvertNodeElseThrow< IParameterized >(node);

        // Retrieve the parameters the node has to compare to list of TypeChecks and convert to
        // IEntities.
        List< IEntity > nodeParameters =
            hasParameters.GetParameters().Select(x => ctx.Graph.Relations.GetEntityByName(x)).ToList();

        List< ICheckResult > subCheckResultsResults = new List< ICheckResult >();

        // Node has no parameters
        if (!nodeParameters.Any())
        {
            return new NodeCheckResult
                   {
                       ChildrenCheckResults = subCheckResultsResults,
                       FeedbackMessage = $"The method has no parameters.",
                       Priority = Priority,
                       DependencyCount = DependencyCount,
                       MatchedNode = node,
                       Check = this,
                   };
        }
        // No TypeChecks were provided
        if (!_parameterTypes.Any())
        {
            return new NodeCheckResult
                   {
                       ChildrenCheckResults = subCheckResultsResults,
                       FeedbackMessage = $"No TypeChecks were provided.",
                       Priority = Priority,
                       DependencyCount = DependencyCount,
                       MatchedNode = node,
                       Check = this,
                   };
        }

        // For each typeCheck, check whether one of the parameters of node has the correct type
        // and if so remove from node parameters list.
        foreach (TypeCheck typeCheck in _parameterTypes)
        {
            bool noneCorrect = true;

            // There are no parameters to check with.   
            if (nodeParameters.Count == 0)
            {
                ICheckResult temp = new LeafCheckResult
                                    {
                                        Priority = Priority,
                                        Correct = false,
                                        FeedbackMessage = "There are less parameters than TypeChecks",
                                        DependencyCount = typeCheck.DependencyCount,
                                        MatchedNode = node,
                                        Check = this,
                                    };
                subCheckResultsResults.Add(temp);
                break;
            }

            for (int x = 0;
                 x < nodeParameters.Count();
                 x++)
            {
                IEntity test = nodeParameters[ x ];
                ICheckResult tempCheck = typeCheck.Check(
                    ctx,
                    nodeParameters.ElementAt(x));

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
                        bool anyTrue = nodeCheckResult.ChildrenCheckResults.OfType<LeafCheckResult>().Any(subCheck => subCheck.Correct);

                        if (anyTrue)
                        {
                            subCheckResultsResults.Add(tempCheck);
                            nodeParameters.RemoveAt(x);
                            noneCorrect = false;
                        }
                        break;
                        
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
                   Priority = Priority,
                   DependencyCount = DependencyCount,
                   MatchedNode = node,
                   Check = this,
               };
    }
}
