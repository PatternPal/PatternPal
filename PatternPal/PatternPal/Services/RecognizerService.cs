namespace PatternPal.Services;

/// <inheritdoc cref="Protos.RecognizerService"/>
public class RecognizerService : Protos.RecognizerService.RecognizerServiceBase
{
    // The threshold for always showing a result. Results with a score below the threshold are only
    // shown if the user presses the 'show all' button in the UI.
    private const int ScoreThresholdForShowAll = 80;

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
        List< RecognitionResult > results = (List< RecognitionResult >)runner.Run();

        // Sort results by score, in descending order.
        results.Sort(
            (
                x,
                y) => y.Result.GetScore().CompareTo(x.Result.GetScore()));

        foreach (RecognitionResult result in results)
        {
            // KNOWN: We sorted the results above, so if we only want to return the top results
            // (this is controlled by request.ShowAllResults), we can stop as soon as we encounter a
            // result which has a score below the threshold.
            if (result.Result.GetScore() < ScoreThresholdForShowAll
                && !request.ShowAllResults)
            {
                break;
            }

            // Convert the result types returned by the recognizer to the result types which can be
            // sent over the wire.
            RecognizeResult res = new()
                                  {
                                      Recognizer = result.Pattern.RecognizerType,
                                      ClassName = result.EntityNode.GetFullName(),
                                      Score = (uint)result.Result.GetScore()
                                  };

            foreach (Recognizers.Abstractions.ICheckResult checkResult in result.Result.GetResults())
            {
                res.Results.Add(CreateCheckResult(checkResult));
            }

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
    /// Converts an <see cref="Recognizers.Abstractions.ICheckResult"/> to a <see cref="CheckResult"/>, which can be sent
    /// over the wire.
    /// </summary>
    /// <param name="checkResult">The <see cref="Recognizers.Abstractions.ICheckResult"/> to convert.</param>
    /// <returns>The <see cref="CheckResult"/> instance created from the given <see cref="Recognizers.Abstractions.ICheckResult"/>.</returns>
    private CheckResult CreateCheckResult(
        Recognizers.Abstractions.ICheckResult checkResult)
    {
        CheckResult newCheckResult = new()
                                     {
                                         FeedbackType = (CheckResult.Types.FeedbackType)((int)checkResult.GetFeedbackType() + 1),
                                         Hidden = checkResult.IsHidden,
                                         Score = checkResult.GetScore(),
                                         FeedbackMessage = ResourceUtils.ResultToString(checkResult),
                                     };
        foreach (Recognizers.Abstractions.ICheckResult childCheckResult in checkResult.GetChildFeedback())
        {
            newCheckResult.SubCheckResults.Add(CreateCheckResult(childCheckResult));
        }
        return newCheckResult;
    }
}
