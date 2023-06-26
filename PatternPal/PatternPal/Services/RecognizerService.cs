#region

using IMethod = PatternPal.SyntaxTree.Abstractions.Members.IMethod;

#endregion

namespace PatternPal.Services;

/// <inheritdoc cref="Protos.RecognizerService"/>
public class RecognizerService : Protos.RecognizerService.RecognizerServiceBase
{
    /// <inheritdoc />
    public override Task< GetSupportedRecognizersResponse > GetSupportedRecognizers(
        GetSupportedRecognizersRequest request,
        ServerCallContext context)
    {
        GetSupportedRecognizersResponse response = new();

        foreach (Recognizer supportedRecognizer in RecognizerRunner.SupportedRecognizers.Keys)
        {
            response.Recognizers.Add(supportedRecognizer);
        }

        return Task.FromResult(response);
    }

    /// <inheritdoc />
    public override Task Recognize(
        RecognizeRequest request,
        IServerStreamWriter< RecognizeResponse > responseStream,
        ServerCallContext context)
    {
        // Get the list of files on which to run the recognizers.
        IList< string > ? files = GetFiles(request);
        if (files is null)
        {
            return Task.CompletedTask;
        }

        // Run the recognizers.
        RecognizerRunner runner = new(
            files,
            request.Recognizers );
        IList< RecognizerRunner.RunResult > results = runner.Run();

        // KNOWN: The root check result is always a NodeCheckResult.
        foreach (RecognizerRunner.RunResult runResult in results)
        {
            RecognizeResult rootResult = new()
                                         {
                                             Recognizer = runResult.RecognizerType!.Value
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
            foreach (string requirement in runResult.Requirements!)
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
                if (entityResult.Requirements.Count == 0)
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
                _ => "Not enough requirements correctly implemented"
            };

            // Threshold for when a result has enough requirements correct to be shown to the user.
            const int PERCENTAGE_CORRECT_TRESHOLD = 50;

            // Skip result when it doesn't have the required percentage of correct requirements.
            if (rootResult.PercentageCorrectResults >= PERCENTAGE_CORRECT_TRESHOLD)
            {
                RecognizeResponse response = new()
                                             {
                                                 Result = rootResult
                                             };
                responseStream.WriteAsync(response);
            }
        }

        return Task.CompletedTask;

        // Calculates the number of correct requirements of the given result.
        static (int CorrectRequirements, int NrOfRequirements) CalcCorrectPercentage(
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

    /// <summary>
    /// Group the results into top-level requirements and their sub-requirements.
    /// </summary>
    /// <param name="resultsOrderedByRequirements"><see cref="Result"/>s ordered by their requirement.</param>
    /// <returns>The top-level requirements.</returns>
    private IEnumerable< EntityResult > GroupResultsByRequirement(
        IOrderedEnumerable< Result > resultsOrderedByRequirements)
    {
        // TODO: Deduplicate sub-requirements (e.g. an Any check with both options being 1a.)
        EntityResult ? entityResult = null;
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
                    // Get an entity from the requirements if the entity result doesn't have one yet.
                    if (result.MatchedNode == null)
                    {
                        foreach (Result childResult in entityResult.Requirements)
                        {
                            if (childResult.MatchedNode is {Kind: MatchedNode.Types.MatchedNodeKind.MnkClass or MatchedNode.Types.MatchedNodeKind.MnkInterface})
                            {
                                result.MatchedNode = childResult.MatchedNode;
                                break;
                            }
                        }
                    }

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
            entityResult.Requirements.Add(result);
        }

        if (null != entityResult)
        {
            yield return entityResult;
        }
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
    /// Gets the list of files on which to run the recognizers.
    /// </summary>
    /// <returns>
    /// <see langword="null"/> if the request contains no valid file or project directory, or a list
    /// of files otherwise.
    /// </returns>
    private static IList< string > ? GetFiles(
        RecognizeRequest request)
    {
        switch (request.FileOrProjectCase)
        {
            case RecognizeRequest.FileOrProjectOneofCase.File:
            {
                if (string.IsNullOrWhiteSpace(request.File)
                    || Path.GetExtension(request.File) != ".cs")
                {
                    return null;
                }

                return new List< string >
                       {
                           request.File
                       };
            }
            case RecognizeRequest.FileOrProjectOneofCase.Project:
            {
                if (string.IsNullOrWhiteSpace(request.Project))
                {
                    return null;
                }

                string ? projectDirectory = Path.GetDirectoryName(request.Project);
                if (string.IsNullOrWhiteSpace(projectDirectory))
                {
                    return null;
                }

                return Directory.GetFiles(
                    projectDirectory,
                    "*.cs",
                    SearchOption.AllDirectories).ToList();
            }
            case RecognizeRequest.FileOrProjectOneofCase.None:
            default:
            {
                return null;
            }
        }
    }
}
