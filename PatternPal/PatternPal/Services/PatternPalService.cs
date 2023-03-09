namespace PatternPal.Services;

public class PatternPalService : Protos.PatternPal.PatternPalBase
{
    public override async Task Recognize(
        RecognizeRequest request,
        IServerStreamWriter< RecognizerResult > responseStream,
        ServerCallContext context)
    {
        await responseStream.WriteAsync(
            new RecognizerResult
            {
                DetectedPattern = "Hello World", ClassName = "Hello Sailor", Score = 100
            });
    }
}
