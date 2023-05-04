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
        
        /// <summary>
        /// Constructor for the LoggerService. This service is responsible for logging events to the database. If any other helper classes are needed, they should be added here as well using dependency injection.
        /// </summary>
        /// <param name="logger">Logger for the service class. Currently this follows the default ASP.NET Core logging configuration, but this can be changed in the Startup.cs file. </param>
        /// <param name="repository"> The repository class that is responsible for communicating with the database. </param>
        public LoggerService(ILogger<LoggerService> logger, EventRepository repository)
        {
            _logger = logger;
            _eventRepository = repository;
        }

        /// <summary>
        /// The main logging method. This method is called by the client to log an event. It's a one-way method, so the client doesn't need to wait for a response and will not return any information.
        /// Furthermore since there is no validation in gRPC, we need to do it manually. This is done by checking the validity of the sessionID, subjectID, event type and the client timestamp. If any of these are invalid, an exception is thrown and the client will receive an error message.
        /// </summary>
        /// <param name="request">All information sent along from the Client required to add a new entry</param>
        /// <param name="context">Server context required</param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
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
