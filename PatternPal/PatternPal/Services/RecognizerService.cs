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
                                             Recognizer = runResult.RecognizerType!.Value,
                                             Feedback = "To be determined" // TODO: Generate feedback
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
                if (!resultsByRequirement.TryGetValue(
                    requirement,
                    out Result ? foundResult))
                {
                    resultsByRequirement[ requirement ] = new Result
                                                          {
                                                              Requirement = requirement,
                                                              Correctness = Result.Types.Correctness.CIncorrect,
                                                          };
                }
            }

            int oldEntityCheck = 0;
            EntityResult entityResult = new();

            foreach (Result result in resultsByRequirement.Values.OrderBy(r => r.Requirement))
            {
                string[ ] splittedRequirement = result.Requirement.Split(". ");
                if (splittedRequirement.Length != 2)
                {
                    // Requirement was not in expected format.
                    throw new ArgumentException($"Requirement '{result.Requirement}' does not have the expected format, format should be '1. Description' for top-level requirements, and '1b. Sub-requirement' for its sub-requirements");
                }

                if (int.TryParse(
                        splittedRequirement[ 0 ],
                        out int newEntityCheck)
                    && oldEntityCheck != newEntityCheck)
                {
                    if (oldEntityCheck > 0)
                    {
                        rootResult.EntityResults.Add(entityResult);
                    }
                    oldEntityCheck = newEntityCheck;

                    entityResult = new EntityResult
                                   {
                                       Name = splittedRequirement[ 1 ],
                                       MatchedNode = result.MatchedNode
                                   };
                }
                else
                {
                    result.Requirement = splittedRequirement[ 1 ];
                    entityResult.Requirements.Add(result);
                }
            }

            if (entityResult.Requirements.Count > 0)
            {
                rootResult.EntityResults.Add(entityResult);
            }

            // Get an entity from the requirements if the entity result doesn't have one yet.
            foreach (EntityResult result in rootResult.EntityResults)
            {
                if (result.MatchedNode == null)
                {
                    foreach (Result childResult in result.Requirements)
                    {
                        if (childResult.MatchedNode is {Kind: MatchedNode.Types.MatchedNodeKind.MnkClass or MatchedNode.Types.MatchedNodeKind.MnkInterface})
                        {
                            result.MatchedNode = childResult.MatchedNode;
                            break;
                        }
                    }
                }
            }

            RecognizeResponse response = new()
                                         {
                                             Result = rootResult
                                         };
            responseStream.WriteAsync(response);
        }

        return Task.CompletedTask;
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
                                    Correctness = Result.Types.Correctness.CCorrect,
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

                result.Correctness = resultToProcess.Score.PercentageOf(resultToProcess.PerfectScore) switch
                {
                    100 => Result.Types.Correctness.CCorrect,
                    > 50 => Result.Types.Correctness.CPartiallyCorrect,
                    _ => Result.Types.Correctness.CIncorrect,
                };

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
