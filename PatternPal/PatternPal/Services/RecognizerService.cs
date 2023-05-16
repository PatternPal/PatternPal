using ICheckResult = PatternPal.Core.ICheckResult;

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

        bool initialValue = true;
        foreach (Recognizer recognizer in Enum.GetValues< Recognizer >())
        {
            // Skip the first value of the enum. This value is required by the Protocol Buffer
            // spec, but it shouldn't be used directly, because it's impossible to know whether
            // the value was set to the default value, or no value was set (because when no
            // value is set, the default value is used).
            if (initialValue)
            {
                initialValue = false;
                continue;
            }

            response.Recognizers.Add(recognizer);
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
        ICheckResult ? result = runner.RunV2();
        if (result is null)
        {
            return Task.CompletedTask;
        }

        // KNOWN: The root check result is always a NodeCheckResult.
        NodeCheckResult rootCheckResult = (NodeCheckResult)result;

        foreach (ICheckResult childCheckResult in rootCheckResult.ChildrenCheckResults)
        {
            RecognizeResult res = new()
                                  {
                                      Recognizer = Recognizer.Singleton,
                                      ClassName = childCheckResult.MatchedNode?.GetName() ?? "no matched node"
                                  };

            res.Results.Add(CreateCheckResult(childCheckResult));

            RecognizeResponse response = new()
                                         {
                                             Result = res
                                         };
            responseStream.WriteAsync(response);
        }

        return Task.CompletedTask;
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
                                         FeedbackType = (CheckResult.Types.FeedbackType.FeedbackCorrect),
                                         Hidden = false,
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
            case LeafCheckResult:
            default:
            {
                // A LeafCheckResult doesn't have any sub-checks.
                break;
            }
        }

        return newCheckResult;
    }
}
