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

        //TODO: Instead of sending random data to the Logging Server, use the data of receivedRequest.
        //Send request to logging server
        LogRequest sendRequest = new LogRequest
        {
            //Random data
            EventType = EventType.SessionStart,
            EventId = "312-1067-B31D-214124124",
            SubjectId = "6B29FC40-CA47-1067-B31D-00DD010662DA",
            ToolInstances = "vs",
            CodeStateId = "213421-2123",
            ClientTimestamp = "2023-04-03T20:07:49+0000",
            ExperimentalCondition = "213421-2123",
            LoggingErrorId = "213421-2123",
            ParentEventId = "213421-2123",
            SessionId = "6B29FC40-CA47-1067-B31D-00DD010662DA",
            ProjectId = "213421-2123",
            CodeStateSection = "213421-2123",
            EventInitiatior = EventInitiator.UserDirectAction,
            EditType = EditType.GenericEdit,
            CompileResult = "213421-2123",
            CompileMessageType = "213421-2123",
            CompileMessageData = "213421-2123",
            SourceLocation = "213421-2123",
            ExecutionId = "213421-2123",
            TestId = "213421-2123",
            ExecutionResultId = "213421-2123",
            InterventionCategory = "213421-2123",
            InterventionType = "213421-2123",
            InterventionMessage = "213421-2123",


        };
        Log.LogClient client = new Log.LogClient(grpcChannel);
        LogReply logReply = client.Log(sendRequest);

        //Send response back to frond-end to verify something has been received here
        LogEventResponse response = new LogEventResponse { ResponseMessage = "a response message." };
        return Task.FromResult(response);

    }

}
