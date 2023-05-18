#region

using Grpc.Net.Client;
using PatternPal.LoggingServer;
using ExecutionResult = PatternPal.LoggingServer.ExecutionResult;

#endregion

namespace PatternPal.Services;

/*
 * Implementation of contract defined in protocol buffer.
 * What to do with the received log and send to logging server.
 */
public class LoggingService : LogProviderService.LogProviderServiceBase
{
    public override Task<LogEventResponse> LogEvent(LogEventRequest receivedRequest, ServerCallContext context)
    {
        GrpcChannel grpcChannel = GrpcChannel.ForAddress(
            "http://178.128.140.163:8080");

        LogRequest sendRequest = DetermineSpecificLog(receivedRequest);
        LogCollectorService.LogCollectorServiceClient client = new(grpcChannel);
        client.Log(sendRequest);

        // Send response back to frond-end to verify something has been received here
        LogEventResponse response = new() { ResponseMessage = "a response message." };
        return Task.FromResult(response);
    }

    /// <summary>
    /// Returns a log in the right format for the logging server based on the event type.
    /// </summary>
    private static LogRequest DetermineSpecificLog(LogEventRequest receivedRequest)
    {
        return receivedRequest.EventType switch
        {
            Protos.EventType.EvtCompile => CompileLog(receivedRequest),
            Protos.EventType.EvtCompileError => CompileErrorLog(receivedRequest),
            Protos.EventType.EvtProjectOpen => ProjectOpenLog(receivedRequest),
            Protos.EventType.EvtProjectClose => ProjectCloseLog(receivedRequest),
            Protos.EventType.EvtDebugProgram => DebugProgramLog(receivedRequest),
            Protos.EventType.EvtSessionStart => SessionStartLog(receivedRequest),
            Protos.EventType.EvtSessionEnd => SessionEndLog(receivedRequest),
            _ => StandardLog(receivedRequest)
        };
    }


    /// <summary>
    /// Each log has certain required fields that are independent of the EventType. These fields are set here.
    /// </summary>
    /// <param name="receivedRequest"></param>
    /// <returns></returns>
    private static LogRequest StandardLog(LogEventRequest receivedRequest)
    {
        return new LogRequest
        {
            EventId = receivedRequest.EventId, //TO DO: Generate ID in Logging Server self
            SubjectId = receivedRequest.SubjectId,
            ToolInstances = Environment.Version.ToString(),
            CodeStateId = Guid.NewGuid().ToString(),
            ClientTimestamp =
                DateTime.UtcNow.ToString(
                    "yyyy-MM-dd HH:mm:ss.fff zzz"), //TO DO: DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff  zzz"), : Logging server cannot work with offsets yet
            SessionId =
                receivedRequest.SessionId 
        };
    }

    #region Log types
    private static LogRequest CompileLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtCompile;
        sendLog.CompileResult = receivedRequest.CompileResult;
        return sendLog;
    }

    private static LogRequest CompileErrorLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtCompileError;
        sendLog.CompileMessageData = receivedRequest.CompileMessageData;
        sendLog.CompileMessageType = receivedRequest.CompileMessageType;
        sendLog.CodeStateSection = receivedRequest.CodeStateSection;
        sendLog.ParentEventId = receivedRequest.ParentEventId;
        sendLog.SourceLocation = receivedRequest.SourceLocation;
        return sendLog;
    }
    private static LogRequest ProjectCloseLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtProjectClose;
        sendLog.ProjectId = receivedRequest.ProjectId;
        return sendLog;
    }

    private static LogRequest ProjectOpenLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtProjectOpen;
        sendLog.ProjectId = receivedRequest.ProjectId;
        return sendLog;
    }

    private static LogRequest DebugProgramLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtDebugProgram;
        sendLog.ExecutionId = receivedRequest.ExecutionId;
        sendLog.ExecutionResult = (ExecutionResult)receivedRequest.ExecutionResult;

        return sendLog;
    }
    private static LogRequest SessionStartLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtSessionStart;

        return sendLog;
    }
    private static LogRequest SessionEndLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtSessionEnd;
      
        return sendLog;
    }
    #endregion
}



