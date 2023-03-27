using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string EventID { get; set; }
        public string SubjectID { get; set; }
        public string ToolInstances { get; set; }
        public string CodeStateID { get; set; }
        public EventType EventType { get; set; }
    }

}
