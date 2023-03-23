using CsvHelper.Configuration.Attributes;
using Google.Protobuf.WellKnownTypes;
using PatternPal.LoggingServer.Models;

namespace PatternPal.LoggingServer.Models
{
    public enum EventType
    {
        SessionStart,
        SessionEnd,
        ProjectOpen,
        ProjectClose,
        FileCreate,
        FileDelete,
        FileOpen,
        FileClose,
        FileFocus,
        FileRename,
        FileCopy,
        FileEdit,
        Compile,
        CompileError,
        CompileWarning,
        Submit,
        RunProgram,
        DebugProgram,
        RunTest,
        DebugTest,
        ResourceView,
        Intervention,
        Custom
    }

    public class ProgSnap2Event
    {
        [Name("EventID")]
        public string EventID { get; set; }
        [Name("SubjectID")]
        public string SubjectID { get; set; }
        public string ToolInstances { get; set; }
        public string CodeStateID { get; set; }
        public EventType EventType { get; set; }
        public int? Order { get; set; }
        public Timestamp? ServerTimeStamp { get; set; }
        public TimeZoneInfo? ServerTimeZone { get; set; }
        public Timestamp? ClientTimeStamp { get; set; }
        public TimeZoneInfo? ClientTimeZone { get; set; }
    }

}
