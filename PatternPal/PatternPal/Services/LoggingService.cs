#region

using Grpc.Net.Client;
using PatternPal.CommonResources;
using PatternPal.Recognizers.Abstractions;
using PatternPal.Recognizers.Models.Output;
using PatternPal.Protos;
using PatternPal.LoggingServer;
using EditType = PatternPal.LoggingServer.EditType;
using EventInitiator = PatternPal.LoggingServer.EventInitiator;
using EventType = PatternPal.LoggingServer.EventType;
using static System.Runtime.InteropServices.JavaScript.JSType;

#endregion

namespace PatternPal.Services;

/*
 * Implementation of contract defined in protocol buffer.
 * What to do with the received log and send to logging server.
 */
public class LoggingService : Protos.LoggingService.LoggingServiceBase
{
    public override Task<LogEventResponse> LogEvent(LogEventRequest receivedRequest, ServerCallContext context)
    {
        GrpcChannel grpcChannel = GrpcChannel.ForAddress(
            "http://localhost:8001");

        //Send request to logging server
        LogRequest sendRequest = new LogRequest
        {
            EventType = EventType.Compile,
            EventId = Guid.NewGuid().ToString(), //TO DO: Generate ID in Logging Server self
            SubjectId = receivedRequest.SubjectId,
            ToolInstances = Environment.Version.ToString(),
            CodeStateId = Guid.NewGuid().ToString(),
            CodeStateSection = Directory.GetCurrentDirectory(),
            ClientTimestamp = "2023-04-03T20:07:49+0000", //TO DO: DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"), : Logging server cannot work with offsets yet
            EventInitiatior = EventInitiator.UserDirectAction,
            SessionId = "6B29FC40-CA47-1067-B31D-00DD010662DA", //TO DO: This ID should be the EventID of the SessionStart event that initiated the session, or it may be derived independently.
            CompileResult = receivedRequest.CompileResult



        };
        Log.LogClient client = new Log.LogClient(grpcChannel);
        LogReply logReply = client.Log(sendRequest);

        //Send response back to frond-end to verify something has been received here
        LogEventResponse response = new LogEventResponse { ResponseMessage = "a response message." };
        return Task.FromResult(response);

    }

}
