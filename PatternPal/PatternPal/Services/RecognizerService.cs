using PatternPal.Core.Runner;

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
        // TODO CV: Handle error cases

        // Get the list of files on which to run the recognizers.
        IList< string > ? files = GetFiles(request);
        if (files is null)
        {
            return Task.CompletedTask;
        }

        // Run the recognizers. We need to cast the result to a `List` to access the `Sort` method,
        // which is defined on the class, not on the `IList` contract.
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
                                             Feedback = "Goed gedoet"
                                         };

            Dictionary< string, Result > resultsByRequirement = new();

            // Get all requirements for which we have results.
            foreach (Result result in GetResults(runResult.CheckResult))
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
                                                          };
                    continue;
                }
            }

            // TODO: Generate feedback for incorrect/missing requirements.

            foreach (Result result in resultsByRequirement.Values)
            {
                rootResult.Results.Add(result);
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
    /// This method transforms a <see cref="ICheckResult"/> to a <see cref="Result"/>
    /// </summary>
    /// <param name="resultsToProcess"></param>
    /// <param name="checkResult"> The check to transform.</param>
    /// <param name="usedNodes"></param>
    /// <returns></returns>
    private static IEnumerable< Result > GetResults(
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
                                    Requirement = resultToProcess.Check.Requirement
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
                                             Length = sourceLocation.Length
                                         };
                }

                // Handle incorrect results.
                Score perfectScore = resultToProcess.Check.PerfectScore;
                Score actualScore = resultToProcess.Score;

                if (perfectScore.Equals(actualScore))
                {
                    yield return result;
                }
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
                        resultsToProcess.Enqueue(childCheckResult);
                    }
                    continue;
            }
        }
    }

    /// <summary>
    /// Gets the list of files on which to run the recognizers.
    /// </summary>
    /// <returns><see langword="null"/> if the request contains no valid file or project directory,
    /// or a list of files otherwise.</returns>
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
