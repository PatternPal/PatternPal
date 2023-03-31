namespace PatternPal.Services;

public class RecognizerService : Protos.RecognizerService.RecognizerServiceBase
{
    private const int SCORE_THRESHOLD_FOR_SHOW_ALL = 80;

    public override Task< GetSupportedRecognizersResponse > GetSupportedRecognizers(
        GetSupportedRecognizersRequest request,
        ServerCallContext context)
    {
        GetSupportedRecognizersResponse response = new();

        bool initialValue = true;
        foreach (object value in Enum.GetValues(typeof( Recognizer )))
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

            response.Recognizers.Add((Recognizer)value);
        }

        return Task.FromResult(response);
    }

    public override Task Recognize(
        RecognizeRequest request,
        IServerStreamWriter< RecognizeResponse > responseStream,
        ServerCallContext context)
    {
        // TODO CV: Handle error cases
        List< string > ? files = null;
        switch (request.FileOrProjectCase)
        {
            case RecognizeRequest.FileOrProjectOneofCase.File:
            {
                if (string.IsNullOrWhiteSpace(request.File))
                {
                    return Task.CompletedTask;
                }

                // Discriminate files based on file name extension
                if (!request.File.EndsWith(".cs"))
                {
                    //TODO programming language is not compatible add a feedback message for project check for .csproj?
                }

                files = new List< string >
                        {
                            request.File
                        };

                break;
            }
            case RecognizeRequest.FileOrProjectOneofCase.Project:
            {
                if (string.IsNullOrWhiteSpace(request.Project))
                {
                    return Task.CompletedTask;
                }

                string ? projectDirectory = Path.GetDirectoryName(request.Project);
                if (string.IsNullOrWhiteSpace(projectDirectory))
                {
                    return Task.CompletedTask;
                }

                files = Directory.GetFiles(
                    projectDirectory,
                    "*.cs",
                    SearchOption.AllDirectories).ToList();
                break;
            }
        }

        if (files is null)
        {
            return Task.CompletedTask;
        }

        RecognizerRunner runner = new();
        runner.CreateGraph(files);

        List< DesignPattern > patterns = new();
        foreach (Recognizer recognizer in request.Recognizers)
        {
            if (recognizer == Recognizer.Unknown)
            {
                continue;
            }

            patterns.Add(RecognizerRunner.GetDesignPattern(recognizer));
        }

        List< RecognitionResult > results = runner.Run(patterns);

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
            if (result.Result.GetScore() < SCORE_THRESHOLD_FOR_SHOW_ALL
                && !request.ShowAllResults)
            {
                break;
            }

            RecognizeResult res = new()
                                  {
                                      Recognizer = result.Pattern.RecognizerType,
                                      ClassName = result.EntityNode.GetFullName(),
                                      Score = (uint)result.Result.GetScore()
                                  };

            foreach (ICheckResult checkResult in result.Result.GetResults())
            {
                res.Results.Add(CreateCheckResult(checkResult));
            }

            RecognizeResponse response = new()
                                         {
                                             Result = res,
                                         };
            responseStream.WriteAsync(response);
        }

        return Task.CompletedTask;
    }

    private CheckResult CreateCheckResult(
        ICheckResult checkResult)
    {
        CheckResult newCheckResult = new()
                                     {
                                         FeedbackType = (CheckResult.Types.FeedbackType)((int)checkResult.GetFeedbackType() + 1),
                                         Hidden = checkResult.IsHidden,
                                         Score = checkResult.GetScore(),
                                         FeedbackMessage = ResourceUtils.ResultToString(checkResult),
                                     };
        foreach (ICheckResult childCheckResult in checkResult.GetChildFeedback())
        {
            newCheckResult.SubCheckResults.Add(CreateCheckResult(childCheckResult));
        }
        return newCheckResult;
    }
}
