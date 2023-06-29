#region

using IMethod = PatternPal.SyntaxTree.Abstractions.Members.IMethod;

#endregion

namespace PatternPal.Services.Helpers;

/// <summary>
/// Helper class for creating gRPC result types from the internal result types.
/// </summary>
static class ResultsHelper
{
    /// <summary>
    /// Creates a <see cref="RecognizeResult"/> from a <see cref="RecognizerRunner.RunResult"/>.
    /// </summary>
    /// <param name="runResult">The <see cref="RecognizerRunner.RunResult"/> to use.</param>
    /// <returns>The created <see cref="RecognizeResult"/>.</returns>
    internal static RecognizeResult CreateRecognizeResult(
        RecognizerRunner.RunResult runResult)
    {
        RecognizeResult rootResult = new()
                                     {
                                         Recognizer = runResult.RecognizerType
                                     };

        Dictionary< string, Result > resultsByRequirement = new();

        // Get all requirements for which we have results.
        IDictionary< ICheck, ICheckResult > resultsByCheck = new Dictionary< ICheck, ICheckResult >();
        foreach (Result result in GetResults(
            resultsByCheck,
            runResult.CheckResult))
        {
            if (!resultsByRequirement.TryGetValue(
                    result.Requirement,
                    out Result ? existingResult)
                || (existingResult.MatchedNode == null && result.MatchedNode != null))
            {
                resultsByRequirement[ result.Requirement ] = result;
            }
        }

        // Check which requirements have no results (because the results were pruned).
        foreach (string requirement in runResult.Requirements)
        {
            if (!resultsByRequirement.ContainsKey(requirement))
            {
                resultsByRequirement[ requirement ] = new Result
                                                      {
                                                          Requirement = requirement,
                                                          Correctness = Correctness.CIncorrect,
                                                      };
            }
        }

        // Group the requirements.
        foreach (EntityResult entityResult in GroupResultsByRequirement(resultsByRequirement.Values.OrderBy(r => r.Requirement)))
        {
            if (entityResult.Requirements.Count == 0
                && (entityResult.Correctness != Correctness.CCorrect || entityResult.MatchedNode == null))
            {
                continue;
            }

            rootResult.EntityResults.Add(entityResult);
        }

        // Calculate the percentage of requirements which are correct.
        int totalCorrectRequirements = 0;
        int totalNrOfRequirements = 0;
        foreach (EntityResult entityResult in rootResult.EntityResults)
        {
            (int correctRequirements, int nrOfRequirements) = CalcCorrectPercentage(entityResult);

            // Count requirement of the entityResult itself.
            if (entityResult.Correctness == Correctness.CCorrect)
            {
                correctRequirements++;
            }
            nrOfRequirements++;

            totalCorrectRequirements += correctRequirements;
            totalNrOfRequirements += nrOfRequirements;

            entityResult.PercentageCorrectRequirements = (int)(correctRequirements / (float)nrOfRequirements * 100);
        }

        rootResult.PercentageCorrectResults = rootResult.EntityResults.Count == 0
            ? 0
            : (int)(totalCorrectRequirements / (float)totalNrOfRequirements * 100);

        rootResult.Feedback = rootResult.PercentageCorrectResults switch
        {
            100 => $"Well done! You have correctly implemented the {rootResult.Recognizer} design pattern.",
            >= 75 => $"Nearly there! There are a couple requirements left you still need to implement for a correct implementation of the {rootResult.Recognizer} design pattern.",
            >= 50 => $"It looks like you are trying to implement the {rootResult.Recognizer} design pattern, but you are still missing quite a few requirements.",
            _ => "Not enough requirements correctly implemented."
        };

        return rootResult;
    }

    /// <summary>
    /// Converts the <see cref="ICheckResult"/>s to <see cref="Result"/>s, which can be sent to the
    /// frontend.
    /// </summary>
    /// <param name="resultsByCheck">Maps <see cref="ICheck"/>s to their <see cref="ICheckResult"/>s.</param>
    /// <param name="rootCheckResult">The root <see cref="ICheckResult"/>.</param>
    /// <returns><see cref="Result"/>s.</returns>
    private static IEnumerable< Result > GetResults(
        IDictionary< ICheck, ICheckResult > resultsByCheck,
        ICheckResult rootCheckResult)
    {
        Queue< ICheckResult > resultsToProcess = new();
        resultsToProcess.Enqueue(rootCheckResult);
        while (resultsToProcess.Count != 0)
        {
            ICheckResult resultToProcess = resultsToProcess.Dequeue();

            if (!string.IsNullOrWhiteSpace(resultToProcess.Check.Requirement))
            {
                Result result = new()
                                {
                                    Requirement = resultToProcess.Check.Requirement,
                                    Correctness = resultToProcess.Score.PercentageOf(resultToProcess.PerfectScore) switch
                                    {
                                        100 => Correctness.CCorrect,
                                        > 50 => Correctness.CPartiallyCorrect,
                                        _ => Correctness.CIncorrect,
                                    }
                                };

                if (resultToProcess.PerfectScore.Equals(default)
                    && resultToProcess.Check is NodeCheckBase
                    && resultToProcess is NodeCheckResult {NodeCheckCollectionWrapper: false})
                {
                    // ASSUME: This result belongs to a NodeCheck (e.g. a MethodCheck) with no
                    // sub-checks. When the check has sub-checks which are incorrect, the
                    // PerfectScore won't be 0. If the NodeCheck didn't match any nodes, the result
                    // would contain an incorrect LeafCheckResult, resulting in a non-zero
                    // PerfectScore.
                    result.Correctness = Correctness.CCorrect;
                }

                if (resultToProcess.MatchedNode != null)
                {
                    INode matchedNode = resultToProcess.MatchedNode;
                    TextSpan sourceLocation = matchedNode.GetSourceLocation;
                    result.MatchedNode = new MatchedNode
                                         {
                                             Name = matchedNode.GetName(),
                                             Path = matchedNode.GetRoot().GetSource(),
                                             Start = sourceLocation.Start,
                                             Length = sourceLocation.Length,
                                             Kind = resultToProcess.MatchedNode switch
                                             {
                                                 IClass => MatchedNode.Types.MatchedNodeKind.MnkClass,
                                                 IInterface => MatchedNode.Types.MatchedNodeKind.MnkInterface,
                                                 IConstructor => MatchedNode.Types.MatchedNodeKind.MnkConstructor,
                                                 IMethod => MatchedNode.Types.MatchedNodeKind.MnkMethod,
                                                 IField => MatchedNode.Types.MatchedNodeKind.MnkField,
                                                 IProperty => MatchedNode.Types.MatchedNodeKind.MnkProperty,
                                                 _ => MatchedNode.Types.MatchedNodeKind.MnkUnknown,
                                             }
                                         };
                }

                yield return result;
            }

            switch (resultToProcess)
            {
                case LeafCheckResult:
                    continue;
                case NotCheckResult notCheckResult:
                    resultsToProcess.Enqueue(notCheckResult.NestedResult);
                    continue;
                case NodeCheckResult nodeCheckResult:
                    foreach (ICheckResult childCheckResult in nodeCheckResult.ChildrenCheckResults)
                    {
                        if (childCheckResult is NodeCheckResult {NodeCheckCollectionWrapper: true} childNodeCheckResult)
                        {
                            if (childNodeCheckResult.ChildrenCheckResults.FirstOrDefault() is NodeCheckResult matchedNodeCheckResult)
                            {
                                resultsByCheck[ matchedNodeCheckResult.Check ] = matchedNodeCheckResult;
                            }
                        }
                        resultsToProcess.Enqueue(childCheckResult);
                    }
                    continue;
            }
        }
    }

    /// <summary>
    /// Group the results into top-level requirements and their sub-requirements.
    /// </summary>
    /// <param name="resultsOrderedByRequirements"><see cref="Result"/>s ordered by their requirement.</param>
    /// <returns>The top-level requirements.</returns>
    private static IEnumerable< EntityResult > GroupResultsByRequirement(
        IOrderedEnumerable< Result > resultsOrderedByRequirements)
    {
        EntityResult ? entityResult = null;
        Dictionary< string, Result > subResultsByPrefix = new();
        foreach (Result result in resultsOrderedByRequirements)
        {
            string[ ] parts = result.Requirement.Split(". ");
            if (parts.Length != 2)
            {
                // Requirement was not in expected format.
                throw new ArgumentException($"Requirement '{result.Requirement}' does not have the expected format, format should be '1. Description' for top-level requirements, and '1b. Sub-requirement' for its sub-requirements");
            }

            // Check if the requirement is a top-level requirement.
            if (int.TryParse(
                parts[ 0 ],
                out _))
            {
                // If `entityResult` is not null, we have found a new top-level requirement.
                if (null != entityResult)
                {
                    SelectMatchedNode(entityResult);
                    yield return entityResult;
                }

                entityResult = new EntityResult
                               {
                                   Name = parts[ 1 ],
                                   MatchedNode = result.MatchedNode,
                                   Correctness = result.Correctness
                               };
                continue;
            }

            if (null == entityResult)
            {
                throw new ArgumentException($"Missing top-level requirement for '{parts[ 1 ]}'");
            }

            result.Requirement = parts[ 1 ];

            // ASSUME: If there are multiple sub-requirements with the same prefix, only 1 will be
            // correct. Use the one which is correct, or which came first if somehow both are
            // correct.
            //Result ? duplicateRequirement = entityResult.Requirements.FirstOrDefault(r => r.Requirement == result.Requirement);
            if (subResultsByPrefix.TryGetValue(
                parts[ 0 ],
                out Result ? duplicateRequirement))
            {
                if (duplicateRequirement.Correctness > result.Correctness)
                {
                    // Replace the duplicate requirement.
                    entityResult.Requirements[ entityResult.Requirements.IndexOf(duplicateRequirement) ] = result;
                    subResultsByPrefix[ parts[ 0 ] ] = result;
                }
            }
            else
            {
                entityResult.Requirements.Add(result);
                subResultsByPrefix.TryAdd(
                    parts[ 0 ],
                    result);
            }
        }

        if (null != entityResult)
        {
            SelectMatchedNode(entityResult);
            yield return entityResult;
        }

        // Get an entity from the requirements if the entity result doesn't have one yet.
        static void SelectMatchedNode(
            EntityResult entityResult)
        {
            if (entityResult.MatchedNode == null)
            {
                foreach (Result childResult in entityResult.Requirements)
                {
                    if (childResult.MatchedNode is {Kind: MatchedNode.Types.MatchedNodeKind.MnkClass or MatchedNode.Types.MatchedNodeKind.MnkInterface})
                    {
                        entityResult.MatchedNode = childResult.MatchedNode;
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Calculates the number of correct requirements of the given <see cref="EntityResult"/>. 
    /// </summary>
    /// <param name="entityResult">The <see cref="EntityResult"/> from which to calculate the correct requirements.</param>
    /// <returns>The number of correct requirements.</returns>
    private static (int CorrectRequirements, int NrOfRequirements) CalcCorrectPercentage(
        EntityResult entityResult)
    {
        if (entityResult.Requirements.Count == 0)
        {
            return (0, 0);
        }

        int correctRequirements = 0;
        foreach (Result subRequirement in entityResult.Requirements)
        {
            if (subRequirement.Correctness == Correctness.CCorrect)
            {
                correctRequirements++;
            }
        }

        return (correctRequirements, entityResult.Requirements.Count);
    }
}
