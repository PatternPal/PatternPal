#region

using Google.Protobuf;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Grpc.Net.Client;
using PatternPal.LoggingServer;

using ExecutionResult = PatternPal.LoggingServer.ExecutionResult;
using LogStatusCodes = PatternPal.LoggingServer.LogStatusCodes;

#endregion

namespace PatternPal.Services;

/// <summary>
/// Implementation of contract defined in protocol buffer.
/// What to do with the received log and send to logging server.
/// </summary>
public class LoggingService : LogProviderService.LogProviderServiceBase
{
    /// <summary>
    /// Stores the last codeState that has been successfully logged to the server. They
    /// are stored as follows: They are stored under [projectID][fileNameRelativeToProjectDir].
    /// </summary>
    private static readonly Dictionary<string, Dictionary<string, string>> _lastCodeState = new();

    /// <inheritdoc />
    public override Task<LogEventResponse> LogEvent(LogEventRequest receivedRequest, ServerCallContext context)
    {
        (LogRequest request, bool discard) specificLog = DetermineSpecificLog((receivedRequest));
        LogEventResponse taskResult = new();

        if (specificLog.discard)
        {
            return Task.FromResult(taskResult);
        }

        try
        {
            // TODO add ENV variable for server address
            GrpcChannel grpcChannel = GrpcChannel.ForAddress(
                "http://161.35.87.186:8080");
            LogCollectorService.LogCollectorServiceClient client = new(grpcChannel);

            LogResponse response = client.Log(specificLog.request, deadline: DateTime.UtcNow.AddSeconds(3));

            if (response.Status == LogStatusCodes.LscSuccess && specificLog.request.HasData)
            {
                // We only store the logged data as the previous codeState if we are certain it was successfully logged.
                UpdateHistory(specificLog.request.ProjectId, specificLog.request.Data);
            }

            taskResult.Message = response.Message;
            taskResult.Status = (Protos.LogStatusCodes)response.Status;
        }
        catch (RpcException e)
        {
            switch (e.StatusCode)
            {
                case StatusCode.Unavailable:
                case StatusCode.DeadlineExceeded:
                    taskResult.Status = Protos.LogStatusCodes.LscUnavailable;
                    break;
                default:
                    taskResult.Status = Protos.LogStatusCodes.LscFailure;
                    break;
            };
            taskResult.Message = e.Message;
        }

        catch (Exception e)
        {
            taskResult.Status = Protos.LogStatusCodes.LscFailure;
            taskResult.Message = e.Message;
        }

        return Task.FromResult(taskResult);
    }

    /// <summary>
    /// Returns a log in the right format for the logging server based on the event type.
    /// </summary>
    private static (LogRequest request, bool discard) DetermineSpecificLog(LogEventRequest receivedRequest)
    {
        return receivedRequest.EventType switch
        {
            Protos.EventType.EvtCompile => (CompileLog(receivedRequest), false),
            Protos.EventType.EvtCompileError => (CompileErrorLog(receivedRequest), false),
            Protos.EventType.EvtFileCreate => (FileCreateLog(receivedRequest), false),
            Protos.EventType.EvtFileDelete => (FileDeleteLog(receivedRequest), false),
            Protos.EventType.EvtFileRename => (FileRenameLog(receivedRequest), false),
            // Currently, the only LogEvent that might need to be discarded is the FileEditEvent, since the file might not have been changed after all.
            Protos.EventType.EvtFileEdit => FileEditLog(receivedRequest),

            Protos.EventType.EvtProjectOpen => (ProjectOpenLog(receivedRequest), false),
            Protos.EventType.EvtProjectClose => (ProjectCloseLog(receivedRequest), false),
            Protos.EventType.EvtDebugProgram => (DebugProgramLog(receivedRequest), false),
            Protos.EventType.EvtSessionStart => (SessionStartLog(receivedRequest), false),
            Protos.EventType.EvtSessionEnd => (SessionEndLog(receivedRequest), false),
            Protos.EventType.EvtXRecognizerRun => (RecognizeLog(receivedRequest), false),
            Protos.EventType.EvtXStepByStepStep => (StepByStepLog(receivedRequest), false),
            _ => (StandardLog(receivedRequest), false)
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
            EventId = receivedRequest.EventId,
            SubjectId = receivedRequest.SubjectId,
            ToolInstances = receivedRequest.ToolInstances,
            ClientTimestamp =
                DateTime.UtcNow.ToString(
                    "yyyy-MM-dd HH:mm:ss.fff zzz"),
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
        // TODO PatternPal only supports running the recognizer on a single project, so the projectID should be set as well.
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
    /// received event and further specific details relevant for the FileCreate-event.
    /// </summary>
    /// <param name="receivedRequest">The originally received request from the PP extension</param>
    /// <returns>A LogRequest populated for this specific event</returns>
    private static LogRequest FileCreateLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);

        sendLog.EventType = LoggingServer.EventType.EvtFileCreate;
        sendLog.CodeStateSection = receivedRequest.CodeStateSection;
        sendLog.ProjectId = receivedRequest.ProjectId;

        string relativePath = Path.GetRelativePath(receivedRequest.ProjectDirectory, receivedRequest.FilePath);
        sendLog.Data = ZipPath(receivedRequest.FilePath, relativePath);
        sendLog.FullCodeState = false;

        return sendLog;
    }

    /// <summary>
    /// Creates a LogRequest that is populated with info obtained from the supplied
    /// received event and further specific details relevant for the FileDelete-event.
    /// </summary>
    /// <param name="receivedRequest">The originally received request from the PP extension</param>
    /// <returns>A LogRequest populated for this specific event</returns>
    private static LogRequest FileDeleteLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);

        sendLog.EventType = LoggingServer.EventType.EvtFileDelete;
        sendLog.CodeStateSection = receivedRequest.CodeStateSection;
        sendLog.ProjectId = receivedRequest.ProjectId;

        string filePathInProjectDir = Path.GetRelativePath(receivedRequest.ProjectDirectory, sendLog.CodeStateSection);

        try
        {
            _lastCodeState[sendLog.ProjectId].Remove(filePathInProjectDir);
        }
        catch
        {
            return OnFailedDictionaryAction(sendLog, receivedRequest.ProjectDirectory);
        }

        return sendLog;
    }

    /// <summary>
    /// Creates a LogRequest that is populated with info obtained from the supplied
    /// received event and further specific details relevant for the FileEdit-event.
    /// </summary>
    /// <param name="receivedRequest">The originally received request from the PP extension</param>
    /// <returns>A tuple of a LogRequest populated for this specific event, and a bool flagging whether the request should be discarded or not</returns>
    private static (LogRequest request, bool discard) FileEditLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);

        string currentHash = HashFile(receivedRequest.FilePath);
        string relativePath = Path.GetRelativePath(receivedRequest.ProjectDirectory, receivedRequest.FilePath);

        sendLog.EventType = LoggingServer.EventType.EvtFileEdit;
        sendLog.CodeStateSection = receivedRequest.CodeStateSection;
        sendLog.ProjectId = receivedRequest.ProjectId;

        // We try to obtain the previous hash of the file to compare it to the new one;
        // if a lookup fails, we reupload the entire codeState.
        {
            string oldHash;
            try
            {
                oldHash = _lastCodeState[receivedRequest.ProjectId][relativePath];
            }
            catch
            {
                return (OnFailedDictionaryAction(sendLog, receivedRequest.ProjectDirectory), false);
            }

            if (currentHash == oldHash)
            {
                return (sendLog, true);
            }
        }

        sendLog.Data = ZipPath(receivedRequest.FilePath, relativePath);
        sendLog.FullCodeState = false;

        return (sendLog, false);
    }


    /// <summary>
    /// Creates a LogRequest that is populated with info obtained from the supplied
    /// received event and further specific details relevant for the FileRename-event.
    /// </summary>
    /// <param name="receivedRequest">The originally received request from the PP extension</param>
    /// <returns>A LogRequest populated for this specific event</returns>
    private static LogRequest FileRenameLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);
        sendLog.EventType = LoggingServer.EventType.EvtFileRename;
        sendLog.CodeStateSection = receivedRequest.CodeStateSection;
        sendLog.ProjectId = receivedRequest.ProjectId;

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
            sendLog.Data = ZipPath(receivedRequest.FilePath);
            sendLog.FullCodeState = true;
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
            sendLog.Data = ZipPath(receivedRequest.FilePath);
            sendLog.FullCodeState = true;
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
        // TODO Should include ProjectID
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

    /// <summary>
    /// Creates a LogRequest that is populated with info obtained from the supplied StepByStep event. This log is ran whenever the client presses check during the step by step process.
    /// </summary>
    /// <param name="receivedRequest">Request received from extension</param>
    /// <returns>A logRequest populated for Step-By-Step event</returns>
    private static LogRequest StepByStepLog(LogEventRequest receivedRequest)
    {
        LogRequest sendLog = StandardLog(receivedRequest);

        sendLog.EventType = LoggingServer.EventType.EvtXStepByStepStep;
        sendLog.RecognizerConfig = receivedRequest.RecognizerConfig;
        sendLog.RecognizerResult = receivedRequest.RecognizerResult;
        sendLog.ProjectId = receivedRequest.ProjectId;
        
        return sendLog;
    }

    #endregion

    #region Utils

    /// <summary>
    /// When path is a directory: zips only the *.cs-files in the supplied path to an in-memory archive,
    /// while retaining the directory structure relative to the directory as root.
    /// When path is a file: zips that file to an in-memory archive, placing it in the directory structure
    /// as supplied by the optional parameter relativePath.
    /// </summary>
    /// <param name="path">The absolute path to the file or directory</param>
    /// <param name="relativePath">Optional: the path to the relative parent directory of the file</param>
    /// <returns>A ByteString of the resulting archive</returns>
    public static ByteString ZipPath(string path, string relativePath = "")
    {
        // Note: Calculating dirSize in C# is not trivial; we are currently not protecting against large codeBases.

        bool isDirectory = Directory.Exists(path);
        Byte[] bytes;

        // This will match things like /bin/, bin/, etc. and make sure
        // those are excluded from the archive.
        Regex rgx = new Regex(@"(^|(\\|/))((bin)|(obj))((\\|/))");

        using (MemoryStream ms = new MemoryStream())
        {
            using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Create))
            {
                if (isDirectory)
                {
                    // If the supplied path is a directory, we enumerate all *.cs-files in the directory and add it to the archive.
                    string[] files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        // We need the filePath relative to the directory that is being zipped to prevent inclusion
                        // of the entire directory structure on the user's disk.
                        string relativeFile = Path.GetRelativePath(path, file);

                        if (rgx.IsMatch(relativeFile))
                        {
                            continue;
                        }

                        // Note that we open the file using the full path, but we create the entry using
                        // the relative path to prevent the entire directory structure from being incorporated
                        // in the archive.
                        ZipArchiveEntry entry = archive.CreateEntry(relativeFile, CompressionLevel.Optimal);

                        using Stream entryStream = entry.Open();
                        using FileStream contents = File.OpenRead(file);
                        contents.CopyTo(entryStream);
                    }
                }

                else
                {
                    // If the supplied path wasn't a directory it is assumed to be a path; since only direct edits
                    // of specific *.cs-files will trigger this branch, we also do not check whether this
                    // file is a *.cs-file or whether it is part of a bin/obj-folder.

                    // Note that the relativePath to the file needs to be supplied via the optional argument here.
                    ZipArchiveEntry entry = archive.CreateEntry(relativePath, CompressionLevel.Optimal);

                    using Stream entryStream = entry.Open();
                    using FileStream contents = File.OpenRead(path);
                    contents.CopyTo(entryStream);
                }
            }

            bytes = ms.ToArray();
        }

        // The final memoryStream containing the zip archive is represented as an array of bytes;
        //  we copy it to a byteString here in order to facilitate easy gRPC usage.
        return ByteString.CopyFrom(bytes);
    }

    /// <summary>
    /// Generates a MD5 hash of the file specified at path.
    /// </summary>
    /// <param name="path">Path to the file to hash</param>
    /// <returns>A base64-string representation of the hash</returns>
    private static string HashFile(string path)
    {
        using MD5 md5 = MD5.Create();
        using FileStream stream = File.OpenRead(path);
        return Convert.ToBase64String(md5.ComputeHash(stream));
    }

    /// <summary>
    /// Populates _lastCodeState by traversing the supplied archive in data and
    /// hashing all entries.
    /// </summary>
    /// <param name="projectID"></param>
    /// <param name="data"></param>
    private static void UpdateHistory(string projectID, ByteString data)
    {
        byte[] compressed = data.ToArray();

        using MemoryStream ms = new MemoryStream(compressed);
        using ZipArchive archive = new ZipArchive(ms);
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            // We try and add an entry per projectID to create new ones
            // when a project was not yet included.
            _lastCodeState.TryAdd(projectID, new Dictionary<String, String>());

            using MD5 md5 = MD5.Create();
            using Stream stream = entry.Open();

            // The lookup on key projectID will always succeed since we created that entry a few lines above.    
            _lastCodeState[projectID][entry.FullName] = Convert.ToBase64String(md5.ComputeHash(stream));
        }

    }

   /// <summary>
   /// Tries to resolve a failed dictionary action (like a lookup) by reuploading the entire codeBase
   /// and thus also repopulating the stored lastCodeState for the current project.
   /// </summary>
   /// <param name="sendLog">The LogRequest as populated thus far</param>
   /// <param name="projectDirectory">Path to the project directory</param>
   /// <returns></returns>
    private static LogRequest OnFailedDictionaryAction(LogRequest sendLog, string projectDirectory)
   {
       // We explicitly remove the currently stored lastCodeState for the project.
       _lastCodeState.Remove(sendLog.ProjectId);

       sendLog.Data = ZipPath(projectDirectory);
       sendLog.FullCodeState = true;

       return sendLog;
   }
    
    #endregion
}
