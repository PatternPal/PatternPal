using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PatternPal.LoggingServer.Models;

namespace PatternPal.LoggingServer.Models
{

    public class ProgSnap2Event
    {
        // Primary key for events, This must be UNIQ
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EventId { get; set; }


        // Order of event in session, starting at 0
        public int Order { get; set; }

        // ID of the session this event belongs to. So every extension launch is a new session.
        public Guid SessionId { get; set; }

        // ID of the subject this event belongs to. So every new extension installation is a new subject.
        public Guid SubjectId { get; set; }

        // Infomration about the IDE, Compiler, OS, Programming lanuage etc.
        public string ToolInstances { get; set; }

        // Refers to folder where the code state can be found. 
        public Guid CodeStateId { get; set; }

        public EventType EventType { get; set; }

        // Datetime of event considering the server timezone
        public DateTimeOffset ServerDatetime { get; set; }

        // Datetime of event considering the client timezone.
        public DateTimeOffset ClientDatetime { get; set; }

    }

}
