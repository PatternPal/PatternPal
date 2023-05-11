using System.IO.Compression;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using PatternPal.LoggingServer;
using PatternPal.LoggingServer.Data;
using PatternPal.LoggingServer.Data.Interfaces;
using PatternPal.LoggingServer.Models;
namespace PatternPal.LoggingServer.Services
{
    public class LoggerService : LogCollectorService.LogCollectorServiceBase
    {
        private readonly ILogger<LoggerService> _logger;
        private readonly EventRepository _eventRepository;
        public LoggerService(ILogger<LoggerService> logger, EventRepository repository)
        {
            _logger = logger;
            _eventRepository = repository;
        }

        public override async Task<LogResponse> Log(LogRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.SessionId, out Guid sessionId))
            {
                Status status = new Status(StatusCode.InvalidArgument, "Invalid sessionID GUID format");

                throw new RpcException(status);
            }

            if (!Guid.TryParse(request.SubjectId, out Guid subjectId))
            {
                Status status = new Status(StatusCode.InvalidArgument, "Invalid subjectID GUID format");
                throw new RpcException(status);
            }

            Guid parentEventId = Guid.Empty;
            if (request.ParentEventId != "" && !Guid.TryParse(request.ParentEventId, out parentEventId))
            {
                Status status = new Status(StatusCode.InvalidArgument, "Invalid parentEventID GUID format");
                throw new RpcException(status);
            }
            if (request.EventType == EventType.EvtUnknown)
            {
                Status status = new Status(StatusCode.InvalidArgument, "Unknown event type");
                throw new RpcException(status);
            }

            if (!DateTimeOffset.TryParse(request.ClientTimestamp, out DateTimeOffset cDto))
            {
                Status status = new Status(StatusCode.InvalidArgument, "Invalid datetime format ( ISO 8601 ) ");
                throw new RpcException(status);
            }

            
            Guid codeStateId = Guid.NewGuid();
            if (request.HasData)
            {
                byte[] compressed = request.Data.ToByteArray();
                string basePath = Path.Combine(Directory.GetCurrentDirectory(), "CodeStates");
                string codeStatePath = Path.Combine(basePath, codeStateId.ToString());
                // Create the directory if it does not exist
                if (!Directory.Exists(codeStatePath))
                {
                    Directory.CreateDirectory(codeStatePath);
                }

                // Convert the byte array to a zip file and extract it
                using (MemoryStream ms = new MemoryStream(compressed))
                {
                    using (ZipArchive archive = new ZipArchive(ms))
                    {

                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            string fullPath = Path.Combine(codeStatePath, entry.FullName);
                            string? directory = Path.GetDirectoryName(fullPath);
                            // Create the directory if it does not exist (for nested directories). This does not need to check for existence because it will just continue if it does exist already.4
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
                    }
                }
            }


            
            int order = await _eventRepository.GetNextOrder(sessionId, subjectId);

            ProgSnap2Event newEvent = new ProgSnap2Event
            {
                Order = order, 
                EventType = request.EventType,
                EventId = Guid.NewGuid(),
                SubjectId = subjectId,
                ToolInstances = request.ToolInstances,
                CodeStateId = codeStateId,
                ClientDatetime = cDto,
                ServerDatetime = DateTimeOffset.Now,
                SessionId = sessionId,
                ProjectId = request.ProjectId,
                ParentId = parentEventId,
                CompileMessage = request.CompileMessageData,
                CompileMessageType = request.CompileMessageType,
                SourceLocation = request.SourceLocation,
                CodeStateSection = request.CodeStateSection

            };

            await _eventRepository.Insert(newEvent);

            return await Task.FromResult(new LogResponse
            {
                Message = "Logged"
            });
        }
    }
}
