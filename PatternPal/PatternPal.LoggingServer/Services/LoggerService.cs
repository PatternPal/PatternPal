#region

using System.IO.Compression;
using Grpc.Core;
using PatternPal.LoggingServer.Data;
using PatternPal.LoggingServer.Models;

#endregion

namespace PatternPal.LoggingServer.Services
{
    public class LoggerService : LogCollectorService.LogCollectorServiceBase
    {
        private readonly ILogger<LoggerService> _logger;
        private readonly EventRepository _eventRepository;

        private readonly string _codeStateDirectory;
        
        /// <summary>
        /// Constructor for the LoggerService. This service is responsible for logging events to the database. If any other helper classes are needed, they should be added here as well using dependency injection.
        /// </summary>
        /// <param name="logger">Logger for the service class. Currently this follows the default ASP.NET Core logging configuration, but this can be changed in the Startup.cs file. </param>
        /// <param name="repository"> The repository class that is responsible for communicating with the database. </param>
        public LoggerService(ILogger<LoggerService> logger, EventRepository repository)
        {
            _logger = logger;
            _eventRepository = repository;
            _codeStateDirectory = Path.Combine(Directory.GetCurrentDirectory(), "CodeStates");
        }

        /// <summary>
        /// The main logging method. This method is called by the client to log an event. It's a one-way method, so the client doesn't need to wait for a response and will not return any information.
        /// Furthermore since there is no validation in gRPC, we need to do it manually. This is done by checking the validity of the sessionID, subjectID, event type and the client timestamp. If any of these are invalid, an exception is thrown and the client will receive an error message.
        /// </summary>
        /// <param name="request">All information sent along from the Client required to add a new entry</param>
        /// <param name="context">Server context required</param>
        /// <returns cref="LogResponse">Confirmation</returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<LogResponse> Log(LogRequest request, ServerCallContext context)
        {
            // GUID parsing
            Guid eventId = GetGuid(request.EventId, "EventID");
            Guid sessionId = GetGuid(request.SessionId, "SessionID");
            string subjectId = request.SubjectId;
            Guid? parentEventId = request.HasParentEventId ? Guid.Parse(request.ParentEventId) : null;

            if (!DateTimeOffset.TryParse(request.ClientTimestamp, out DateTimeOffset cDto))
            {
                Status status = new Status(StatusCode.InvalidArgument, "Invalid datetime format ( ISO 8601 ) ");
                throw new RpcException(status);
            }

            Guid? codeStateId = null;
            if (request.HasData)
            {
                codeStateId = ParseCodeState(request.Data.ToByteArray());
            }

            int order = await _eventRepository.GetNextOrder(sessionId, subjectId);

            ProgSnap2Event newEvent = new ProgSnap2Event
            {
                Order = order, 
                EventType = request.EventType,
                EventId = eventId,
                SubjectId = subjectId,
                ToolInstances = request.ToolInstances,
                CodeStateId = codeStateId,
                FullCodeState = request.HasFullCodeState ? request.FullCodeState : null,
                ClientDatetime = cDto,
                ServerDatetime = DateTimeOffset.Now,
                SessionId = sessionId,
                ProjectId = request.HasProjectId ? request.ProjectId : null,
                ParentId = parentEventId,
                CompileMessage = request.HasCompileMessageData ? request.CompileMessageData : null,
                CompileMessageType = request.HasCompileMessageType ? request.CompileMessageType: null,
                SourceLocation = request.HasSourceLocation ? request.SourceLocation: null,
                CodeStateSection = request.HasCodeStateSection ? request.CodeStateSection : null,
                RecognizerConfig = request.EventType == EventType.EvtXRecognizerRun ? request.RecognizerConfig : null,
                RecognizerResult = request.EventType == EventType.EvtXRecognizerRun ? request.RecognizerResult : null,
                ExecutionResult = request.HasExecutionResult ? request.ExecutionResult : null
            };

            // For atomic transactions, it would be a good future improvement to delete the respective codeState-directory
            // if inserting in the database fails. For now, this is also caught by running the housekeeping-job.
            await _eventRepository.Insert(newEvent);

            return await Task.FromResult(new LogResponse
            {
                Message = "Logged"
            });
        }

        /// <summary>
        /// Unpacks the supplied compressed data to their respective codeState-subdirectory.
        /// </summary>
        /// <param name="compressed">A bytestring representing the compressed data</param>
        /// <returns>The CodeState-GUID</returns>
        /// <exception cref="RpcException">Is thrown when the supplied bytestring was not a valid zip archive.</exception>
        private Guid ParseCodeState(byte[] compressed)
        {
            Guid codeStateId = Guid.NewGuid();

            if (!IsZipArchive(compressed))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Data is not a valid zip archive"));
            }

            string codeStatePath = Path.Combine(_codeStateDirectory, codeStateId.ToString());
            Directory.CreateDirectory(codeStatePath);

            // Convert the byte array to a zip file and extract it
            using MemoryStream ms = new(compressed);
            using ZipArchive archive = new(ms);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string fullPath = Path.Combine(codeStatePath, entry.FullName);
                string? directory = Path.GetDirectoryName(fullPath);
                // Create the directory if it does not exist (for nested directories)
                if (directory != null)
                {
                    Directory.CreateDirectory(directory);
                }

                // Skip non-cs files
                if (!entry.FullName.EndsWith(".cs"))
                {
                    continue;
                }

                entry.ExtractToFile(fullPath, true);
            }

            return codeStateId;
        }

        /// <summary>
        /// Helper function to shorten the log function by parsing a GUID from a string and throwing an exception if it is not valid
        /// </summary>
        /// <param name="guidString">The actual string that needs to be parsed</param>
        /// <param name="guidName">When throwing exception this is the name of the GUID that was being parsed</param>
        /// <returns cref="Guid">Parsed Guid</returns>
        /// <exception cref="RpcException">Exception when parsing fails</exception>
        private static Guid GetGuid(string guidString, string guidName)
        {
            if (Guid.TryParse(guidString, out Guid guid))
            {
                return guid;
            }

            Status status = new Status(StatusCode.InvalidArgument, $"Invalid {guidName} GUID format");
            throw new RpcException(status);
        }

        /// <summary>
        /// Checks if the supplied byte array is a valid zip archive
        /// </summary>
        /// <param name="compressed">The byte array possibly representing a zip archive</param>
        /// <returns>Whether the archive is a valid zip archive, or not</returns>
        private bool IsZipArchive(byte[] compressed)
        {
            try
            {
                using MemoryStream ms = new MemoryStream(compressed);
                using ZipArchive archive = new ZipArchive(ms);
                return true;
            }
            catch (InvalidDataException)
            {
                return false;
            }
        }
    }
}
