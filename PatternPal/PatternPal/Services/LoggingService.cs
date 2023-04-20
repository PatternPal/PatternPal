#region

using Grpc.Net.Client;
using PatternPal.LoggingServer;
#endregion

namespace PatternPal.Services;

/*
 * Implementation of contract defined in protocol buffer.
 * What to do with the received log and send to logging server.
 */
public class LoggingService : Protos.LogProviderService.LogProviderServiceBase
{
    public override Task<LogEventResponse> LogEvent(LogEventRequest receivedRequest, ServerCallContext context)
    {
        GrpcChannel grpcChannel = GrpcChannel.ForAddress(
            "http://localhost:8000");

        LogRequest sendRequest = DetermineSpecificLog(receivedRequest);
        LogCollectorService.LogCollectorServiceClient client = new LogCollectorService.LogCollectorServiceClient(grpcChannel);
        LogResponse logReply = client.Log(sendRequest);

        // Send response back to frond-end to verify something has been received here
        LogEventResponse response = new LogEventResponse { ResponseMessage = "a response message." };
        return Task.FromResult(response);
    }

    /// <summary>
    /// Returns a log in the right format for the logging server based on the event type.
    /// </summary>
    private LogRequest DetermineSpecificLog(LogEventRequest receivedRequest)
    {
        switch (receivedRequest.EventType)
        {
            case Protos.EventType.EvtCompile:
                return CompileLog(receivedRequest);
            case Protos.EventType.EvtCompileError:
                return CompileErrorLog(receivedRequest);
            case Protos.EventType.EvtSessionStart:
                return SessionStartLog(receivedRequest);
            case Protos.EventType.EvtSessionEnd:
                return SessionEndLog(receivedRequest);
            default:
                return StandardLog(receivedRequest);
        }
    }


    /// <summary>
    /// Each log has certain required fields that are independent of the EventType. These fields are set here.
    /// </summary>
    /// <param name="receivedRequest"></param>
    /// <returns></returns>
    private LogRequest StandardLog(LogEventRequest receivedRequest)
    {
        return new LogRequest
        {
            EventId = Guid.NewGuid().ToString(), //TO DO: Generate ID in Logging Server self
            SubjectId = receivedRequest.SubjectId,
            ToolInstances = Environment.Version.ToString(),
            CodeStateSection = Directory.GetCurrentDirectory(),
            CodeStateId = Guid.NewGuid().ToString(),
            ClientTimestamp =
                DateTime.UtcNow.ToString(
                    "yyyy-MM-dd HH:mm:ss.fff zzz"), //TO DO: DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff  zzz"), : Logging server cannot work with offsets yet
            SessionId =
                receivedRequest.SessionId 
        };
    }

    private LogRequest CompileLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtCompile;
        sendLog.CompileResult = receivedRequest.CompileResult;
        return sendLog;
    }

    private LogRequest CompileErrorLog(LogEventRequest receivedRequest)
    {
        //TO DO: add code state from which the compilation error occurred.
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtCompileError;
        return sendLog;
    }

    private LogRequest SessionStartLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtSessionStart;

        return sendLog;
    }


    private LogRequest SessionEndLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtSessionEnd;
      
        return sendLog;
    }
}



