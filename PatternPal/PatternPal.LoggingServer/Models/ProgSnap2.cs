using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PatternPal.LoggingServer.Models;
using PatternPal.LoggingServer;
namespace PatternPal.LoggingServer.Models
{
    public class ProgSnap2Event
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EventId { get; set; }

        public int Order { get; set; }
        public Guid SessionId { get; set; }
        public Guid SubjectId { get; set; }
        public string ToolInstances { get; set; }
        public Guid CodeStateId { get; set; }
        public EventType EventType { get; set; }

        public Guid? ParentId { get; set; }
        public string? ProjectId { get; set; }

        public string? CompileMessageType { get; set; } 

        public string? CompileMessage { get; set; }
        public string? SourceLocation { get; set; }

        public string? CodeStateSection { get; set; }

        public DateTimeOffset ServerDatetime { get; set; }
        public DateTimeOffset ClientDatetime { get; set; }

        public ExecutionResult? ExecutionResult { get; set; }

    }

}
