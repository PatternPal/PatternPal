#region

using PatternPal.CommonResources;
using PatternPal.Recognizers.Abstractions;
using PatternPal.Recognizers.Models.Output;

#endregion

namespace PatternPal.Services;

/*
 * Implementation of contract defined in protocol buffer.
 * What to do with the received log and send to logging server for Hieke.
 */
public class LoggingService : Protos.LoggingService.LoggingServiceBase
{
    public override Task<LogBuildEventResponse> LogBuildEvent(LogBuildEventRequest request, ServerCallContext context)
    {
        //Do something with the request 
        LogBuildEventResponse response = new LogBuildEventResponse();
        return Task.FromResult(response);

    }

}
