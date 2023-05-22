#region

using Google.Protobuf;
using System.IO.Compression;
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
    /// <summary>
    /// Stores the last codeState that has been successfully logged to the server, per projectId.
    /// </summary>
    private static Dictionary<String, ByteString> _lastCodeState = new Dictionary<string, ByteString>();

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="receivedRequest">TODO</param>
    /// <param name="context">TODO</param>
    /// <returns></returns>
    public override Task<LogEventResponse> LogEvent(LogEventRequest receivedRequest, ServerCallContext context)
    {
        GrpcChannel grpcChannel = GrpcChannel.ForAddress(
            "http://178.128.140.163:8080");

        LogRequest sendRequest = DetermineSpecificLog(receivedRequest);
        LogCollectorService.LogCollectorServiceClient client = new(grpcChannel);

        // TODO What should be done with the actual response of the logging server?
        LogResponse res = client.Log(sendRequest); 

        // TODO First check if logging was successful
        if (sendRequest.HasData)
        {
            _lastCodeState.Add(sendRequest.ProjectId, sendRequest.Data);
        }

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
            EventId = receivedRequest.EventId, //TODO: Generate ID in Logging Server self
            SubjectId = receivedRequest.SubjectId,
            ToolInstances = Environment.Version.ToString(),
            ClientTimestamp =
                DateTime.UtcNow.ToString(
                    "yyyy-MM-dd HH:mm:ss.fff zzz"), //TODO: DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff  zzz"), : Logging server cannot work with offsets yet
            SessionId =
                receivedRequest.SessionId 
        };
    }

    #region Log types
    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="receivedRequest"></param>
    /// <returns></returns>
    private static LogRequest CompileLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtCompile;
        sendLog.CompileResult = receivedRequest.CompileResult;
        return sendLog;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="receivedRequest"></param>
    /// <returns></returns>
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

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="receivedRequest"></param>
    /// <returns></returns>
    private static LogRequest ProjectCloseLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtProjectClose;
        sendLog.ProjectId = receivedRequest.ProjectId;

        if (receivedRequest.HasFilePath)
        {
            sendLog.Data = ZipDirectory(receivedRequest.FilePath);
        }

        return sendLog;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="receivedRequest"></param>
    /// <returns></returns>
    private static LogRequest ProjectOpenLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtProjectOpen;
        sendLog.ProjectId = receivedRequest.ProjectId;

        if (receivedRequest.HasFilePath)
        {
            sendLog.Data = ZipDirectory(receivedRequest.FilePath);
        }

        return sendLog;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="receivedRequest"></param>
    /// <returns></returns>
    private static LogRequest DebugProgramLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtDebugProgram;
        sendLog.ExecutionId = receivedRequest.ExecutionId;
        sendLog.ExecutionResult = (ExecutionResult)receivedRequest.ExecutionResult;

        return sendLog;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="receivedRequest"></param>
    /// <returns></returns>
    private static LogRequest SessionStartLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtSessionStart;

        return sendLog;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="receivedRequest"></param>
    /// <returns></returns>
    private static LogRequest SessionEndLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtSessionEnd;
      
        return sendLog;
    }
    #endregion

    #region Utils
    /// <summary>
    ///  TODO
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static ByteString ZipDirectory(string path)
    {
        // TODO: Protect against too large codebases.
        // TODO: Now also includes *.cs-build artifacts.
        Byte[] bytes;

        using (MemoryStream ms = new MemoryStream())
        {
            using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Create))
            {

                string[] files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    ZipArchiveEntry entry = archive.CreateEntry(GetRelativePath(path, file), CompressionLevel.Optimal);

                    using (Stream entryStream = entry.Open())
                    using (FileStream contents = File.OpenRead(file))
                    {
                        contents.CopyTo(entryStream);
                    }
                }
            }

            bytes = ms.ToArray();
        }

        return ByteString.CopyFrom(bytes);
    }

    // TODO Separate utility because of duplication with extension
    /// <summary>
    /// Gets the relative path when given an absolute directory path and a filename.
    /// </summary>
    /// <param name="relativeTo"> The absolute path to the root folder</param>
    /// <param name="path">The absolute path to the specific file</param>
    /// <returns></returns>
    public static string GetRelativePath(string relativeTo, string path)
    {
        Uri uri = new Uri(relativeTo);
        string rel = Uri.UnescapeDataString(uri.MakeRelativeUri(new Uri(path)).ToString())
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        if (rel.Contains(Path.DirectorySeparatorChar.ToString()) == false)
        {
            rel = $".{Path.DirectorySeparatorChar}{rel}";
        }

        return rel;
    }
    #endregion
}



