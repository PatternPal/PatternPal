#region

using PatternPal.Services.Helpers;

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
            RecognizeResult rootResult = ResultsHelper.CreateRecognizeResult(runResult);

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
