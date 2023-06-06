#region

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace PatternPal.LoggingServer.Models
{
    /// <summary>
    /// Represents an event in ProgSnap2, including information about the session, subject, tool instances, code state, and timing.
    /// </summary>
    public class ProgSnap2Event
    {
        /// <summary>
        /// The primary key for the event. This must be unique.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EventId { get; set; }

        /// <summary>
        /// The order of the event within the session, starting at 0.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The ID of the session that this event belongs to. Every extension launch is a new session.
        /// </summary>
        public Guid SessionId { get; set; }

        /// <summary>
        /// The ID of the subject that this event belongs to. Every new extension installation is a new subject.
        /// </summary>
        public string SubjectId { get; set; }

        /// <summary>
        /// Information about the IDE, compiler, OS, programming language, and other tool instances.
        /// </summary>
        public string ToolInstances { get; set; }

        /// <summary>
        /// The ID of the code state that this event refers to.
        /// </summary>
        public Guid? CodeStateId { get; set; }

        /// <summary>
        /// Whether the stored codeState was complete or partial.
        /// </summary>
        public bool? FullCodeState { get; set;  }

        /// <summary>
        /// The type of event.
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// The date and time of the event, in the server's time zone.
        /// </summary>
        public DateTimeOffset ServerDatetime { get; set; }

        /// <summary>
        /// The date and time of the event, in the client's time zone.
        /// </summary>
        public DateTimeOffset ClientDatetime { get; set; }

        /// <summary>
        /// The ID of the parent event, if any. Used in cases such as compile.error, where the parent event is compile.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// The ID of the project associated. This is generated per project, and is the same for all events in the same project.
        /// </summary>
        public string? ProjectId { get; set; }

        /// <summary>
        /// The type of compile message, such as "error", "warning", or "message".
        /// </summary>
        public string? CompileMessageType { get; set; }

        /// <summary>
        /// The content of the compile message.
        /// </summary>
        public string? CompileMessage { get; set; }

        /// <summary>
        /// The source location associated with this compile event, if any.
        /// </summary>
        public string? SourceLocation { get; set; }

        /// <summary>
        /// The section of the code state associated with this compile event. For example in compile.error, this would be the section of code that caused the error.
        /// </summary>
        public string? CodeStateSection { get; set; }

        /// <summary>
        /// Result in case of debug.run event. Value can be "success", "failure", "timeout", "error", "unknown".
        /// </summary>
        public ExecutionResult? ExecutionResult { get; set; }

        /// <summary>
        /// The results of the recognizer run, if any. This is a JSON string.
        /// </summary>
        public string? RecognizerResult { get; set; }

        /// <summary>
        /// The configuration of the recognizer run, if any. This is a JSON string.
        /// </summary>
        public string? RecognizerConfig { get; set; }
    }
}
