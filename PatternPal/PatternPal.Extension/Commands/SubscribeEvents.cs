#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using EnvDTE;
using EnvDTE80;

using Microsoft.VisualStudio.Shell;

using PatternPal.Extension.Grpc;
using PatternPal.Protos;

using JsonSerializer = System.Text.Json.JsonSerializer;

#endregion

namespace PatternPal.Extension.Commands
{
    public enum ExtensionLogStatusCodes
    {
        Available,
        Unavailable,
        Error,
        NoLog
    }

    /// <summary>
    /// A static class which is responsible for subscribing logged event in ProgSnap2 format.
    /// </summary>
    public static class SubscribeEvents
    {
        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private static ExtensionWindowPackage _package;

        private static DTE _dte;

        private static DebuggerEvents _dteDebugEvents;

        private static DocumentEvents _dteDocumentEvents;

        private static SolutionEvents _dteSolutionEvents;

        private static BuildEvents _dteBuildEvents;

        private static FileSystemWatcher _watcher;

        private static bool _unhandledExceptionThrown;

        public static string SessionId { get; set; }
        public static string SubjectId { get; set; }

        private static bool _doLog = false;

        private static CancellationToken _cancellationToken;

        private static ExtensionLogStatusCodes _serverStatus = ExtensionLogStatusCodes.NoLog;

        private static string _toolInstances = "";
        public static ExtensionLogStatusCodes ServerStatus
        {
            get
            {
                if (_doLog)
                {
                    return _serverStatus;
                }

                return ExtensionLogStatusCodes.NoLog;
            }
            set => _serverStatus = value;
        }

        public static Action ServerStatusChanged = delegate { };

        /// <summary>
        /// Initializes the preparation for the subscription of the logged events. 
        /// </summary>
        /// <param name="dte"></param>
        /// <param name="package"> The PatternPal package itself. </param>
        public static void Initialize(
            DTE dte,
            ExtensionWindowPackage package, CancellationToken cancellationToken)
        {
            _dte = dte;

            ThreadHelper.ThrowIfNotOnUIThread();

            _dteDebugEvents = _dte.Events.DebuggerEvents;
            _dteSolutionEvents = _dte.Events.SolutionEvents;
            _dteBuildEvents = _dte.Events.BuildEvents;
            _dteDocumentEvents = _dte.Events.DocumentEvents;
            _package = package;
            _cancellationToken = cancellationToken;
            _toolInstances = $" { _dte.Version } { _dte.Edition } { _dte.Name } {Vsix.Version}";

            // We should call OnChangedLoggingPreference to "load" a possibly stored setting. The
            // application always stored with the internal flag set to false, so this will only actually
            // do something when it was stored as true (and subsequently kickstart the logging session).
            OnChangedLoggingPreference(Privacy.Instance);
        }

        /// <summary>
        /// Subscribes the event handlers for logging data.
        /// Be careful with calling this method. This method should only be called once.
        /// </summary>
        public static async Task SubscribeEventHandlersAsync()
        {
            await _package.JoinableTaskFactory.SwitchToMainThreadAsync(_cancellationToken);
            // Code that interacts with UI elements goes here
            _dteBuildEvents.OnBuildDone += OnCompileDone;
            _dteSolutionEvents.Opened += OnSolutionOpen;
            _dteSolutionEvents.BeforeClosing += OnSolutionClose;
            _dteDebugEvents.OnEnterBreakMode +=
                OnExceptionUnhandled; // OnEnterBreakMode is triggered for both breakpoints as well as exceptions, with the reason parameter specifying this.
            _dteDebugEvents.OnEnterDesignMode += OnDebugProgram;
            _dteDocumentEvents.DocumentSaved += OnDocumentSaved;
        }


        /// <summary>
        /// Unsubscribes the event handlers for logging data.
        /// </summary>
        public static async Task UnsubscribeEventHandlersAsync()
        {
            await _package.JoinableTaskFactory.SwitchToMainThreadAsync(_cancellationToken);
            _dteBuildEvents.OnBuildDone -= OnCompileDone;
            _dteSolutionEvents.Opened -= OnSolutionOpen;
            _dteSolutionEvents.BeforeClosing -= OnSolutionClose;
            _dteDebugEvents.OnEnterBreakMode -= OnExceptionUnhandled;
            _dteDebugEvents.OnEnterDesignMode -= OnDebugProgram;
            _dteDocumentEvents.DocumentSaved -= OnDocumentSaved;
        }

        /// <summary>
        /// Event handler for a changed logging preference in the privacy screen.
        /// </summary>
        /// <param name="obj"></param>
        public static void OnChangedLoggingPreference(Privacy obj)
        {
            // We explicitly check if the value has changed. 
            if (_doLog == obj.DoLogData)
            {
                return;
            }

            _doLog = obj.DoLogData;

            // If the value changes, a new session is either started or ended.
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await (obj.DoLogData
                    ? SubscribeEventHandlersAsync()
                    : UnsubscribeEventHandlersAsync());
            });

            ThreadHelper.ThrowIfNotOnUIThread();
            if (obj.DoLogData)
            {
                OnSessionStart();
                OnSolutionOpen();
            }
            else
            {
                OnSolutionClose();
                OnSessionEnd();
            }
        }

        #region Events

        /// <summary>
        /// The event handler for handling the Compile Event. The given parameters are part of the event listener input and among other things necessary to give the right output message.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="action"></param>
        private static void OnCompileDone(
            vsBuildScope scope,
            vsBuildAction action)
        {
            // TODO This method needs major refactoring and error handling
            if (action == vsBuildAction.vsBuildActionClean)
            {
                return;
            }

            ThreadHelper.ThrowIfNotOnUIThread(); ;
            string outputMessage = _dte.Solution.SolutionBuild.LastBuildInfo != 0 ? 
                $"Build {action.ToString()} with errors. See the output window for details." : 
                $"Build {action.ToString()} succeeded.";

            LogEventRequest request = CreateStandardLog();

            string pathSolutionFullName = _dte.Solution.FullName;
            string pathSolutionFile = _dte.Solution.FileName;
        
            // Distinguish a sln file or just a csproj file to be opened
            if (pathSolutionFullName == "")
            {
                // A csproj file was opened
                Array startupProjects = (Array)_dte.Solution.SolutionBuild.StartupProjects;
                request.CodeStateSection = (string)startupProjects.GetValue(0);
            }

            else
            {
                string pathSolutionDirectory = Path.GetDirectoryName(pathSolutionFullName);
                request.CodeStateSection = GetRelativePath(pathSolutionDirectory, pathSolutionFile);
            }

            if(scope == vsBuildScope.vsBuildScopeProject)
            {
                // If the build scope was a project and not the entire solution, we know for
                // sure that a start-up project has been specified and thus we can say something useful
                // about the projectId. We chose to only store the first one because building multiple
                // projects seems out of the scope of PatternPal and ProgSnap2.
                Array startupProjects = (Array)_dte.Solution.SolutionBuild.StartupProjects;
                request.ProjectId = (string)startupProjects.GetValue(0);
            }

            request.EventType = EventType.EvtCompile;
            request.CompileResult = outputMessage;

            LogEventResponse response = PushLog(request);

            // When the compilation was an error, a Compile Error log needs to be send.
            if (_dte.Solution.SolutionBuild.LastBuildInfo != 0)
            {
                Window window = _dte.Windows.Item(WindowKinds.vsWindowKindErrorList);
                ErrorList errorListWindow = (ErrorList)window.Selection;

                for (int i = 1; i <= errorListWindow.ErrorItems.Count; i++)
                {
                    ErrorItem errorItem = errorListWindow.ErrorItems.Item(i);
                    string errorType = errorItem.ErrorLevel.ToString();
                    string errorMessage = errorItem.Description;
                    string errorSourceLocation = String.Concat("Text:" + errorItem.Line);

                    // The relative path of the source file for the error is required for the ProgSnap format
                    string projectFolderName =
                        Path.GetDirectoryName(_dte.Solution.Projects.Item(errorItem.Project).FullName);
                    string codeStateSection = GetRelativePath(projectFolderName, errorItem.FileName);

                    OnCompileError(request, errorType, errorMessage, errorSourceLocation, codeStateSection);
                }
            }
        }

        /// <summary>
        /// The event handler for handling the File.Create Event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fileSystemEventArgs"></param>
        internal static void OnFileCreate(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            // Since the fileWatcher will also fire this event for created .cs-files contained in the bin artifacts, we
            // need to filter for those as well.
            Regex rgx = new Regex(@"(^|(\\|/))((bin)|(obj))((\\|/))");
            if (rgx.IsMatch(fileSystemEventArgs.Name))
            {
                return;
            }

            LogEventRequest request = CreateStandardLog();

            request.EventType = EventType.EvtFileCreate;
            request.CodeStateSection = fileSystemEventArgs.Name;

            string projectFullPath = FindContainingCsprojFile(fileSystemEventArgs.FullPath);
            string projectDirectory = Path.GetDirectoryName(projectFullPath);
            request.ProjectId = GetRelativePath(projectDirectory, projectFullPath);
            request.ProjectDirectory = Path.GetDirectoryName(projectFullPath);
            request.ProjectId = GetRelativePath(projectDirectory, projectFullPath);

            request.FilePath = fileSystemEventArgs.FullPath;
            
            LogEventResponse response = PushLog(request);
        }

        /// <summary>
        /// The event handler for handling the File.Delete Event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fileSystemEventArgs"></param>
        internal static void OnFileDelete(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            // Other operations might also cause the fileWatcher to fire this event, so we explicitly check if the file
            // does not exist anymore.
            if (File.Exists(fileSystemEventArgs.FullPath))
            {
                return;
            }

            // Since the fileWatcher will also fire this event for deleted .cs-files contained in the bin artifacts, we
            // need to filter for those as well.
            Regex rgx = new Regex(@"(^|(\\|/))((bin)|(obj))((\\|/))");
            if (rgx.IsMatch(fileSystemEventArgs.Name))
            {
                return;
            }

            LogEventRequest request = CreateStandardLog();

            request.EventType = EventType.EvtFileDelete;
            request.CodeStateSection = fileSystemEventArgs.Name;

            string projectFullPath = FindContainingCsprojFile(fileSystemEventArgs.FullPath);
            string projectDirectory = Path.GetDirectoryName(projectFullPath);
            request.ProjectDirectory = projectDirectory;
            request.ProjectId = GetRelativePath(projectDirectory, projectFullPath);

            LogEventResponse response = PushLog(request);
        }

        /// <summary>
        /// The event handler for handling the File.Rename Event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fileSystemEventArgs"></param>
        internal static void OnFileRename(object sender, RenamedEventArgs e)
        {
            // This event might also be triggered by file edits; we can catch this by filtering based on the file extensions.
            if (Path.GetExtension(e.Name) != ".cs" || Path.GetExtension(e.OldName) != ".cs")
            {
                return;
            }

            LogEventRequest request = CreateStandardLog();

            request.EventType = EventType.EvtFileRename;
            request.CodeStateSection = e.Name;
            request.OldFileName = e.OldName;

            string projectFullPath = FindContainingCsprojFile(e.FullPath);
            string projectDirectory = Path.GetDirectoryName(projectFullPath);
            request.ProjectDirectory = projectDirectory;
            request.ProjectId = GetRelativePath(projectDirectory, projectFullPath);

            LogEventResponse response = PushLog(request);
        }


        /// <summary>
        /// The event handler for handling the Session.Start Event. When a new session starts, a (new) sessionID is generated.
        /// </summary>
        internal static void OnSessionStart()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            SubjectId = Privacy.Instance.SubjectId;
            SessionId = Guid.NewGuid().ToString();

            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtSessionStart;

            LogEventResponse response = PushLog(request);
        }

        /// <summary>
        /// The event handler for handling the Session.End Event. This method needs to be called from the package logic, hence the given internal modifier.
        /// </summary>
        internal static void OnSessionEnd()
        {
            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtSessionEnd;

            LogEventResponse response = PushLog(request);
        }

        /// <summary>
        /// The event handler for handling the Compile.Error Event. This is strictly a complementary event to the regular Compile event for each separate error.
        /// </summary>
        private static void OnCompileError(LogEventRequest parent, string compileMessagetype, string compileMessageData,
            string sourceLocation, string codeStateSection)
        {
            LogEventRequest request = CreateStandardLog();

            request.EventType = EventType.EvtCompileError;
            request.ParentEventId = parent.EventId;
            request.CompileMessageType = compileMessagetype;
            request.CompileMessageData = compileMessageData;
            request.SourceLocation = sourceLocation;
            request.CodeStateSection = codeStateSection;
            request.ProjectId = parent.ProjectId;

            LogEventResponse response = PushLog(request);
        }

        /// <summary>
        /// The event handler for handling the Solution.Open Event.
        /// </summary>
        internal static void OnSolutionOpen()
        {
            LogEachProject(EventType.EvtProjectOpen);

            // After a solution has opened we set up the file watcher for the solution directory. 
            SetUpFileWatcher();
        }

        /// <summary>
        /// The event handler for handling the Solution.Close Event.
        /// </summary>
        internal static void OnSolutionClose()
        {
            LogEachProject(EventType.EvtProjectClose);

            // We should dispose of the watcher here; just for safety, we add a null check.
            _watcher?.Dispose();
        }

        /// <summary>
        /// The event handler for handling the Debug.Program Event.
        /// </summary>
        private static void OnDebugProgram(dbgEventReason reason)
        {
            LogEventRequest request = CreateStandardLog();

            request.EventType = EventType.EvtDebugProgram;
            request.ExecutionId = Guid.NewGuid().ToString();

            if (_unhandledExceptionThrown)
            {
                request.ExecutionResult = ExecutionResult.ExtError;
                _unhandledExceptionThrown = false;
            }
            else
            {
                request.ExecutionResult = ExecutionResult.ExtSucces;
            }

            LogEventResponse response = PushLog(request);
        }

        /// <summary>
        /// The event handler for handling the File.Edit Event. This fires at every save of a document regardless whether a change
        /// has been made to the file or not. This remains File.Edit, and not File.Save as during the handling of diffs,
        /// it is determined whether an edit has been made and if so, it is sent with the codebase - therefore: File.Edit.
        /// </summary>
        /// <param name="document">The document that is being saved.</param>
        private static void OnDocumentSaved(Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            LogEventRequest request = CreateStandardLog();

            request.EventType = EventType.EvtFileEdit;
            request.ProjectId = document.ProjectItem.ContainingProject.UniqueName;
            request.ProjectDirectory = Path.GetDirectoryName(document.ProjectItem.ContainingProject.FullName);
            request.CodeStateSection = GetRelativePath(request.ProjectDirectory, document.FullName);
            request.FilePath = document.FullName;

            LogEventResponse response = PushLog(request);
        }

        /// <summary>
        /// The event handler for when recognizing software patterns in the extension.
        /// </summary>
        public static void OnPatternRecognized(RecognizeRequest recognizeRequest,
            IList<RecognizeResult> recognizeResults)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_package == null || !Privacy.Instance.DoLogData)
            { 
              return; 
            }
            
            LogEventRequest request = CreateStandardLog();

            request.EventType = EventType.EvtXRecognizerRun;
            string config = recognizeRequest.Recognizers.ToString();

            request.RecognizerConfig = config;
            foreach (RecognizeResult result in recognizeResults)
            {
                request.RecognizerResult += result.ToString();
            }

            LogEventResponse response = PushLog(request);
        }
        /// <summary>
        /// Function to push a step by step event to the server. It compiles all required information and sends it to the server.
        /// </summary>
        /// <param name="recognizer">Recognizer being used by step-by-step </param>
        /// <param name="currentInstructionNumber">Current instruction number </param>
        /// <param name="result">Boolean output</param>
        /// <param name="documents">all documents being checked against</param>
        public static void OnStepByStepCheck(string recognizer, int currentInstructionNumber, bool result)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!_doLog)
            {
                return;
            }
            LogEventRequest request = CreateStandardLog();

            request.EventType = EventType.EvtXStepByStepStep;
            // config should be dict with recognizer name and current instruction number
            Dictionary<string,string> config = new Dictionary<string, string>()
            {
                { "recognizer", recognizer },
                { "currentInstructionNumber", currentInstructionNumber.ToString()}
            };

            request.RecognizerConfig = JsonSerializer.Serialize(config);
            Dictionary<string,string> results = new Dictionary<string, string>()
            {
                { "result", result.ToString() }
            };

            request.RecognizerResult = JsonSerializer.Serialize(results);
            
            Project project = _dte.ActiveDocument.ProjectItem.ContainingProject;
            request.ProjectId = project.UniqueName;

            LogEventResponse response = PushLog(request);
        }
        #endregion

        /// <summary>
        /// Creates a standard log format with set fields that are always generated. Consequently, it is used by all other specific logs.
        /// </summary>
        /// <returns>The standard log format.</returns>
        private static LogEventRequest CreateStandardLog()
        {
            return new LogEventRequest
            {
                EventId = Guid.NewGuid().ToString(), SubjectId = SubjectId, SessionId = SessionId, ToolInstances = _toolInstances
            };
        }

        /// <summary>
        /// Sends the log request to the background service in order to be processed to the server.
        /// </summary>
        private static LogEventResponse PushLog(LogEventRequest request)
        {
            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
            try
            {
                LogEventResponse ler = client.LogEvent(request);
                ServerStatusChanged?.Invoke();
                switch (ler.Status)
                {
                    case LogStatusCodes.LscSuccess:
                        ServerStatus = ExtensionLogStatusCodes.Available;
                        return ler;

                    case LogStatusCodes.LscUnavailable:
                        ServerStatus = ExtensionLogStatusCodes.Unavailable;
                        return ler;

                    case LogStatusCodes.LscFailure:
                    case LogStatusCodes.LscRejected:
                    case LogStatusCodes.LscInvalidArguments:
                    case LogStatusCodes.LscUnknown:
                    default:
                        ServerStatus = ExtensionLogStatusCodes.Error;
                        return ler;
                }
            }
            // Host not found, timeout exception, etc.
            catch (Exception e)
            {
                ServerStatus = ExtensionLogStatusCodes.Unavailable;
                return new LogEventResponse
                {
                    Status = LogStatusCodes.LscUnknown,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// Gets the relative path when given an absolute directory path and a filename.
        /// </summary>
        /// <param name="relativeTo"> The absolute path to the root folder</param>
        /// <param name="path">The absolute path to the specific file</param>
        public static string GetRelativePath(string relativeTo, string path)
        {
            Uri uri = new Uri(relativeTo);
            string rel = Uri.UnescapeDataString(uri.MakeRelativeUri(new Uri(path)).ToString())
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            if (rel.Contains(Path.DirectorySeparatorChar.ToString()) == false)
            {
                rel = $".{Path.DirectorySeparatorChar}{rel}";
            }
            
            return rel;
        }

        /// <summary>
        /// Cycles through all active projects and log the given event for each of these projects.
        /// </summary>
        private static void LogEachProject(EventType eventType)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Projects projects = _dte.Solution.Projects;

            foreach (Project project in projects)
            {
                // This catches miscellaneous projects without a defined project.Fullname; the cause of this is unknown since
                // we are using internal-use-only libraries for event catching.
                if (project?.FullName == null || !File.Exists((project.FullName)))
                {
                    continue;
                }

                LogEventRequest request = CreateStandardLog();
                request.EventType = eventType;

                // Since LogEachProject is only called by either Project.Open or Project.Close,
                // we include the full CodeState.
                request.ProjectId = project.UniqueName;
                request.FilePath = Path.GetDirectoryName(project.FullName);

                LogEventResponse response = PushLog(request);
            }
        }

        /// <summary>
        /// Sets up a file watcher for File.Create and File.Delete events for the current opened user solution.
        /// </summary>
        private static void SetUpFileWatcher()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // We dispose of the current fileWatcher if it had already been set and not properly been disposed off.
            _watcher?.Dispose();

            // Create new watcher, subscribe events and set properties
            string solutionDirectory = Path.GetDirectoryName(_dte.Solution.FullName);
            if (solutionDirectory == null || !Directory.Exists(solutionDirectory))
            {
                // We shouldn't set up the watcher if the solutionDirectory is null or non-existent; however, it is known to be not null here, so this is is just a safety measure.
                return;
            }
            _watcher = new FileSystemWatcher(solutionDirectory, "*.cs");
            
            _watcher.Created += OnFileCreate;
            _watcher.Deleted += OnFileDelete; 
            _watcher.Renamed += OnFileRename;

            _watcher.EnableRaisingEvents = true;
            _watcher.IncludeSubdirectories = true;
        }

        /// <summary>
        /// Traverse the directory structure to find the .csproj file that contains the given file path.
        /// </summary>
        /// <param name="filePath">The full filepath of the target file. </param>
        /// <returns>The full path of the found .csproj file.</returns>
        private static string FindContainingCsprojFile(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);

            while (!string.IsNullOrEmpty(directory))
            {
                string csprojFile = Directory.GetFiles(directory, "*.csproj").FirstOrDefault();

                if (!string.IsNullOrEmpty(csprojFile))
                {
                    return csprojFile;
                }

                directory = Path.GetDirectoryName(directory);
            }

            return "";
        }

        /// <summary>
        /// Event handler for when an exception is unhandled. This is used to determine in the user's last debug session
        /// whether there were any unhandled exceptions. Although creating a separate method might seem redundant
        /// for the actual logic used here, it is necessary for the adding and removing from any used event listeners.
        /// </summary>
        public static void OnExceptionUnhandled(dbgEventReason reason, ref dbgExecutionAction executionAction)
        {
            if (reason == dbgEventReason.dbgEventReasonExceptionNotHandled)
            {
                _unhandledExceptionThrown = true;
            }
        }
    }
}
