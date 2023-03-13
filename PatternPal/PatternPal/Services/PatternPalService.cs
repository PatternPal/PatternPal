using PatternPal.Recognizers.Abstractions;

namespace PatternPal.Services;

public class PatternPalService : Protos.PatternPal.PatternPalBase
{
    public override Task Recognize(
        RecognizeRequest request,
        IServerStreamWriter< RecognizerResult > responseStream,
        ServerCallContext context)
    {
        Console.WriteLine("Received recognize request: " + request);

        // TODO CV: Handle error cases
        if (string.IsNullOrWhiteSpace(request.File)
            && string.IsNullOrWhiteSpace(request.Project))
        {
            return Task.CompletedTask;
        }

        RecognizerRunner runner = new();
        List< string > files = new()
                               {
                                   request.File,
                               };
        runner.CreateGraph(files);

        List< DesignPattern > patterns = new();
        foreach (Recognizer recognizer in request.Recognizers)
        {
            patterns.Add(
                RecognizerRunner.DesignPatterns.Find(
                    p => string.Equals(
                        p.Name,
                        recognizer.ToString().Replace(
                            "_",
                            " "),
                        StringComparison.InvariantCultureIgnoreCase))!);
        }

        foreach (RecognitionResult result in runner.Run(patterns))
        {
            Result res = new()
                         {
                             Score = result.Result.GetScore()
                         };
            foreach (ICheckResult checkResult in result.Result.GetResults())
            {
                res.Results.Add(CreateCheckResult(checkResult));
            }

            responseStream.WriteAsync(
                new RecognizerResult
                {
                    DetectedPattern = result.Pattern.Name, ClassName = result.EntityNode.GetFullName(), Result = res,
                });
        }

        return Task.CompletedTask;
    }

    private CheckResult CreateCheckResult(
        ICheckResult checkResult)
    {
        CheckResult newCheckResult = new()
                                     {
                                         FeedbackType = (Protos.FeedbackType)((int)checkResult.GetFeedbackType() + 1), Hidden = checkResult.IsHidden, Score = checkResult.GetScore()
                                     };
        foreach (ICheckResult childCheckResult in checkResult.GetChildFeedback())
        {
            newCheckResult.ChildFeedback.Add(CreateCheckResult(childCheckResult));
        }
        return newCheckResult;
    }
}
