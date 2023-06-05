#region

using Google.Protobuf;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Grpc.Net.Client;
using PatternPal.LoggingServer;
using ExecutionResult = PatternPal.LoggingServer.ExecutionResult;

#endregion

namespace PatternPal.Services;

/// <summary>
/// Implementation of contract defined in protocol buffer.
/// What to do with the received log and send to logging server.
/// </summary>
public class LoggingService : LogProviderService.LogProviderServiceBase
{
    /// <summary>
    /// Stores the last codeState that has been successfully logged to the server, per projectId.
    /// </summary>
    private static Dictionary<String, (ByteString CodeState, bool Full)> _lastCodeState =
        new Dictionary<string, (ByteString CodeState, bool Full)>();

    /// <inheritdoc />
    public override Task<LogEventResponse> LogEvent(LogEventRequest receivedRequest, ServerCallContext context)
    {
        GrpcChannel grpcChannel = GrpcChannel.ForAddress(
            "http://161.35.87.186:8080");

        LogRequest sendRequest = DetermineSpecificLog(receivedRequest);
        LogCollectorService.LogCollectorServiceClient client = new(grpcChannel);

        // TODO What should be done with the actual response of the logging server?
        LogResponse res = client.Log(sendRequest);

        // TODO First check if logging was successful
        if (sendRequest.HasData)
        {
            // TODO Whether the codeState is full should eventually be determined here, but for now; always true.
            _lastCodeState[sendRequest.ProjectId] = (sendRequest.Data, true);
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
            Protos.EventType.EvtFileCreate => FileCreateLog(receivedRequest),
            Protos.EventType.EvtFileEdit => FileEditLog(receivedRequest),
            Protos.EventType.EvtProjectOpen => ProjectOpenLog(receivedRequest),
            Protos.EventType.EvtProjectClose => ProjectCloseLog(receivedRequest),
            Protos.EventType.EvtDebugProgram => DebugProgramLog(receivedRequest),
            Protos.EventType.EvtSessionStart => SessionStartLog(receivedRequest),
            Protos.EventType.EvtSessionEnd => SessionEndLog(receivedRequest),
            Protos.EventType.EvtXRecognizerRun => RecognizeLog(receivedRequest),
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
    /// Creates a LogRequest that is populated with info obtained from the supplied
    /// received event and further specific details relevant for the RecognizerRun-event.
    /// </summary>
    /// <param name="receivedRequest">The originally received request from the PP extension</param>
    /// <returns>A LogRequest populated for this specific event</returns>
    private static LogRequest RecognizeLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtXRecognizerRun;
        sendLog.RecognizerResult = receivedRequest.RecognizerResult;
        sendLog.RecognizerConfig = receivedRequest.RecognizerConfig;
        return sendLog;
    }

    /// <summary>
    /// Creates a LogRequest that is populated with info obtained from the supplied
    /// received event and further specific details relevant for the Compile-event.
    /// </summary>
    /// <param name="receivedRequest">The originally received request from the PP extension</param>
    /// <returns>A LogRequest populated for this specific event</returns>
    private static LogRequest CompileLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtCompile;
        sendLog.CompileResult = receivedRequest.CompileResult;
        return sendLog;
    }

    /// <summary>
    /// Creates a LogRequest that is populated with info obtained from the supplied
    /// received event and further specific details relevant for the CompileError-event.
    /// </summary>
    /// <param name="receivedRequest">The originally received request from the PP extension</param>
    /// <returns>A LogRequest populated for this specific event</returns>
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
    /// Creates a LogRequest that is populated with info obtained from the supplied
    /// received event and further specific details relevant for the FileEdit-event.
    /// </summary>
    /// <param name="receivedRequest">The originally received request from the PP extension</param>
    /// <returns>A LogRequest populated for this specific event</returns>
    private static LogRequest FileCreateLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtFileCreate;
        sendLog.CodeStateSection = receivedRequest.CodeStateSection;

        return sendLog;
    }

    /// <summary>
    /// Creates a LogRequest that is populated with info obtained from the supplied
    /// received event and further specific details relevant for the FileEdit-event.
    /// </summary>
    /// <param name="receivedRequest">The originally received request from the PP extension</param>
    /// <returns>A LogRequest populated for this specific event</returns>
    private static LogRequest FileEditLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtFileEdit;
        sendLog.CodeStateSection = receivedRequest.CodeStateSection;

        return sendLog;
    }

    /// <summary>
    /// Creates a LogRequest that is populated with info obtained from the supplied
    /// received event and further specific details relevant for the ProjectClose-event.
    /// </summary>
    /// <param name="receivedRequest">The originally received request from the PP extension</param>
    /// <returns>A LogRequest populated for this specific event</returns>
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
    /// Creates a LogRequest that is populated with info obtained from the supplied
    /// received event and further specific details relevant for the ProjectOpen-event.
    /// </summary>
    /// <param name="receivedRequest">The originally received request from the PP extension</param>
    /// <returns>A LogRequest populated for this specific event</returns>
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
    /// Creates a LogRequest that is populated with info obtained from the supplied
    /// received event and further specific details relevant for the DebugProgram-event.
    /// </summary>
    /// <param name="receivedRequest">The originally received request from the PP extension</param>
    /// <returns>A LogRequest populated for this specific event</returns>
    private static LogRequest DebugProgramLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtDebugProgram;
        sendLog.ExecutionId = receivedRequest.ExecutionId;
        sendLog.ExecutionResult = (ExecutionResult)receivedRequest.ExecutionResult;

        return sendLog;
    }

    /// <summary>
    /// Creates a LogRequest that is populated with info obtained from the supplied
    /// received event and further specific details relevant for the SessionStart-event.
    /// </summary>
    /// <param name="receivedRequest">The originally received request from the PP extension</param>
    /// <returns>A LogRequest populated for this specific event</returns>
    private static LogRequest SessionStartLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtSessionStart;

        return sendLog;
    }

    /// <summary>
    /// Creates a LogRequest that is populated with info obtained from the supplied
    /// received event and further specific details relevant for the SessionEnd-event.
    /// </summary>
    /// <param name="receivedRequest">The originally received request from the PP extension</param>
    /// <returns>A LogRequest populated for this specific event</returns>
    private static LogRequest SessionEndLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtSessionEnd;

        return sendLog;
    }

    #endregion

    #region Utils

    /// <summary>
    /// Zips only the *.cs-files in the supplied path to an in-memory archive,
    /// while retaining the directory structure.
    /// </summary>
    /// <param name="path">The absolute path to the directory</param>
    /// <returns>A ByteString of the resulting archive</returns>
    public static ByteString ZipDirectory(string path)
    {
        // TODO: Protect against too large codebases.
        Byte[] bytes;

        // This will match things like /bin/, bin/, etc. and make sure
        // those are excluded from the archive.
        Regex rgx = new Regex(@"(^|(\\|/))((bin)|(obj))((\\|/))");

        using (MemoryStream ms = new MemoryStream())
        {
            using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Create))
            {
                string[] files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    string relativePath = Path.GetRelativePath(path, file);

                    // If the 
                    if (rgx.IsMatch(relativePath))
                    {
                        continue;
                    }

                    // Note that we open the file using the full path, but we create the entry using
                    // the relative path to prevent the entire directory structure from being incorporated
                    // in the archive.
                    ZipArchiveEntry entry = archive.CreateEntry(relativePath, CompressionLevel.Optimal);

                    using (Stream entryStream = entry.Open())
                    using (FileStream contents = File.OpenRead(file))
                    {
                        contents.CopyTo(entryStream);
                    }
                }
            }

            bytes = ms.ToArray();
        }

        // The final memoryStream containing the zip archive is represented as an array of bytes;
        //  we copy it to a byteString here in order to facilitate easy gRPC usage.
        return ByteString.CopyFrom(bytes);
    }

    #endregion
}
