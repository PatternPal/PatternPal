using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PatternPal.LoggingServer.Models;

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

        public DateTimeOffset ServerDatetime { get; set; }
        public DateTimeOffset ClientDatetime { get; set; }

    }

}
