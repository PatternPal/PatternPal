using PatternPal.Core.Models;

namespace PatternPal.Services;

public class PatternPalService : Protos.PatternPal.PatternPalBase
{
    public override Task Recognize(
        RecognizeRequest request,
        IServerStreamWriter< RecognizerResult > responseStream,
        ServerCallContext context)
    {
        Console.WriteLine("Received recognize request");

        RecognizerRunner runner = new();
        List< string > files = new()
                               {
                                   request.File,
                               };
        runner.CreateGraph(files);

        List< DesignPattern > patterns = new()
                                         {
                                             RecognizerRunner.DesignPatterns.Find(p => p.Name == "Singleton")!
                                         };
        foreach (RecognitionResult result in runner.Run(patterns))
        {
            responseStream.WriteAsync(
                new RecognizerResult
                {
                    DetectedPattern = result.Pattern.Name, ClassName = result.EntityNode.GetFullName(), Score = result.Result.GetScore(),
                });
        }

        return Task.CompletedTask;
    }
}
