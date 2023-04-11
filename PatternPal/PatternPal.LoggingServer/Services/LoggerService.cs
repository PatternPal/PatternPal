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

            Guid sessionId = Guid.Parse(request.SessionId);
            Guid subjectId = Guid.Parse(request.SubjectId);

            DateTimeOffset cDto = DateTimeOffset.Parse(request.ClientTimestamp);
            
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
