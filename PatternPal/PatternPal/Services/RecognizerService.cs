﻿namespace PatternPal.Services;

public class RecognizerService : Protos.RecognizerService.RecognizerServiceBase
{
    public override Task Recognize(
        RecognizeRequest request,
        IServerStreamWriter< RecognizeResponse > responseStream,
        ServerCallContext context)
    {
        string pathFile = request.File;
        string pathProject = request.Project;

        // TODO CV: Handle error cases
        if (string.IsNullOrWhiteSpace(pathFile)
            && string.IsNullOrWhiteSpace(pathProject))
        {
            return Task.CompletedTask;
            //TODO feedback message that no file was selected 
        }

        // Discriminate files based on file name extension
        if (!pathFile.EndsWith(".cs"))
        {
            //TODO programming language is not compatible add a feedback message for project check for .csproj?
        }

        List< string > files;
        // Return all the .cs files (and in all subdirectories)
        // Does not include files part of project but not in directory
        if (!string.IsNullOrEmpty(pathProject))
        {
            files = Directory.GetFiles(
                Path.GetDirectoryName(pathProject),
                "*.cs",
                SearchOption.AllDirectories).ToList();
        }
        else
        {
            files = new List< string >
                    {
                        request.File
                    };
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

        foreach (RecognitionResult result in runner.Run(patterns))
        {
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