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

            byte[] compressed = request.Data.ToByteArray();

            // TODO: File processing (decompression, diff comparison, etc.)

            
            int order = await _eventRepository.GetNextOrder(sessionId, subjectId);

            ProgSnap2Event newEvent = new ProgSnap2Event
            {
                Order = order, 
                EventType = request.EventType,
                EventId = Guid.NewGuid(),
                SubjectId = subjectId,
                ToolInstances = request.ToolInstances,
                CodeStateId = Guid.NewGuid(), // TODO: implement code state
                ClientDatetime = cDto,
                ServerDatetime = DateTimeOffset.Now,
                SessionId = sessionId
            };

            await _eventRepository.Insert(newEvent);

            return await Task.FromResult(new LogResponse
            {
                Message = "Logged"
            });
        }
    }
}
