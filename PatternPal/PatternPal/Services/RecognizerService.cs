﻿namespace PatternPal.Services;

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
        IList< (Recognizer, ICheckResult) > results = runner.Run();

        // KNOWN: The root check result is always a NodeCheckResult.
        foreach ((Recognizer recognizer, ICheckResult rootCheckResult) in results)
        {
            RecognizeResult rootResult = new()
                                         {
                                             Recognizer = recognizer,
                                             Feedback = "Goed gedoet"
                                         };

            Dictionary< string, Result > resultsByRequirement = new();
            foreach (Result result in GetResults(rootCheckResult))
            {
                if (!resultsByRequirement.TryGetValue(
                        result.Requirement,
                        out Result ? existingResult)
                    || (existingResult.MatchedNode == null && result.MatchedNode != null))
                {
                    resultsByRequirement[ result.Requirement ] = result;
                }
            }

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

    private static IEnumerable< Result > GetResults(
        ICheckResult checkResult)
    {
        if (!string.IsNullOrWhiteSpace(checkResult.Check.Requirement))
        {
            Result result = new()
                            {
                                Requirement = checkResult.Check.Requirement
                            };

            if (checkResult.MatchedNode != null)
            {
                result.MatchedNode = new MatchedNode
                                     {
                                         Name = checkResult.MatchedNode.GetName()
                                     };
            }

            yield return result;
        }

        switch (checkResult)
        {
            case LeafCheckResult:
                yield break;
            case NotCheckResult notCheckResult:
            {
                foreach (Result result in GetResults(notCheckResult.NestedResult))
                {
                    yield return result;
                }
                yield break;
            }
            case NodeCheckResult nodeCheckResult:
            {
                foreach (ICheckResult childCheckResult in nodeCheckResult.ChildrenCheckResults)
                {
                    foreach (Result result in GetResults(childCheckResult))
                    {
                        yield return result;
                    }
                }
                yield break;
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

    /// <summary>
    /// Converts an <see cref="ICheckResult"/> to a <see cref="CheckResult"/>, which can be sent
    /// over the wire.
    /// </summary>
    /// <param name="checkResult">The <see cref="ICheckResult"/> to convert.</param>
    /// <returns>The <see cref="CheckResult"/> instance created from the given <see cref="ICheckResult"/>.</returns>
    private static CheckResult CreateCheckResult(
        ICheckResult checkResult)
    {
        CheckResult newCheckResult = new()
                                     {
                                         Requirement = checkResult.Check.Requirement ?? string.Empty,
                                         FeedbackMessage = checkResult.FeedbackMessage
                                     };

        // Convert sub-check results.
        switch (checkResult)
        {
            case NodeCheckResult nodeCheckResult:
            {
                foreach (ICheckResult childCheckResult in nodeCheckResult.ChildrenCheckResults)
                {
                    newCheckResult.SubCheckResults.Add(CreateCheckResult(childCheckResult));
                }
                break;
            }
            case NotCheckResult notCheckResult:
            {
                newCheckResult.SubCheckResults.Add(CreateCheckResult(notCheckResult.NestedResult));
                break;
            }
        }

        return newCheckResult;
    }
}
