using Grpc.Core;
using Microsoft.Extensions.Logging;
using PatternPal.LoggingServer;
using PatternPal.LoggingServer.Data.Interfaces;
using PatternPal.LoggingServer.Models;

namespace PatternPal.LoggingServer.Services
{
    public class LoggerService : Log.LogBase
    {
        private readonly ILogger<LoggerService> _logger;
        private readonly IRepository<ProgSnap2Event> _eventRepository;
        public LoggerService(ILogger<LoggerService> logger, IRepository<ProgSnap2Event> repository)
        {
            _logger = logger;
            _eventRepository = repository;
        }

        public override async Task<LogReply> Log(LogRequest request, ServerCallContext context)
        {
            _logger.LogCritical("skitta");

            var res = await _eventRepository.GetAll();

            _logger.LogCritical(res.Count().ToString());
            _logger.LogInformation("Received Log Request: {0}", request.EventType);

            ProgSnap2Event newEvent = new ProgSnap2Event
            {
                EventType = Models.EventType.Compile,
                EventID = Guid.NewGuid().ToString(),
                SubjectID = request.SubjectID,
                ToolInstances = request.ToolInstances,
                CodeStateID = request.ToolInstances
            };

            await _eventRepository.Insert(newEvent);

            return await Task.FromResult(new LogReply
            {
                Message = "Logged"
            });
        }
    }
}
