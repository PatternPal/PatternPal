namespace PatternPal.Services;

public class PatternPalService : Protos.PatternPal.PatternPalBase
{
    public override Task Recognize(
        RecognizeRequest request,
        IServerStreamWriter< RecognizerResult > responseStream,
        ServerCallContext context)
    {
        return Task.FromResult(
            new RecognizerResult
            {
                DetectedPattern = "Hello World", ClassName = "Hello Sailor", Score = 100
            });
    }
}
