using Grpc.Core;
using PatternPal.LoggingServer;

namespace PatternPal.LoggingServer.Services
{
    public class LoggerService : Log.LogBase
    {
        private readonly ILogger<LoggerService> _logger;
        public LoggerService(ILogger<LoggerService> logger)
        {
            _logger = logger;
        }

        public override Task<LogReply> Log(LogRequest request, ServerCallContext context)
        {
            
            // TODO EVERYTHING

            return Task.FromResult(new LogReply
            {
                Message = "Logged"
            });
        }
    }
}
